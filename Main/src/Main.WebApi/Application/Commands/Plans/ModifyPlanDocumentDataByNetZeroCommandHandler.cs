using System.Text.Json.Nodes;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.Organization;
using Main.Dto.ViewModel.Plan;
using Main.Dto.ViewModel.SystemSetting;
using Main.WebApi.Application.Models.NetZero;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.SystemSetting;
using Main.WebApi.Application.Queries.Tenants;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// Modify PlanDetail Cycle Command Handler
/// </summary>
/// <remarks>
/// First, it modifies the cycle of the specified PlanDetail.
/// Second, it archives the PlanDocument related to the modified PlanDetail.
/// Third, if IsApplyAll is true, it modifies the cycle of all PlanDetails with the same CycleType.
/// Finally, it modifies the StartDate and EndDate of the PlanDocuments related to the modified PlanDetails.
/// </remarks>
/// <param name="logger"></param>
/// <param name="mediator"></param>
/// <param name="planDetailQuery"></param>
/// <param name="userService"></param>
/// <param name="apiConnectionQuery"></param>
/// <param name="planQuery"></param>
/// <param name="httpClientFactory"></param>
/// <param name="planDocumentDataSplitedQuery"></param>
/// <param name="planDocumentDataRepository"></param>
/// <param name="planRepository"></param>
public class ModifyPlanDocumentDataByNetZeroCommandHandler(
    ILogger<ModifyPlanDocumentDataByNetZeroCommand> logger,
    IPlanDetailQuery planDetailQuery,
    IUserService<Scuser> userService,
    IApiConnectionQuery apiConnectionQuery,
    IPlanQuery planQuery,
    ITenantQuery tenantQuery,
    IPlanFactoryQuery planFactoryQuery,
    IHttpClientFactory httpClientFactory,
    IPlanDocumentDataSplitedQuery planDocumentDataSplitedQuery,
    IPlanDocumentDataRepository planDocumentDataRepository,
    IPlanRepository planRepository) : IRequestHandler<ModifyPlanDocumentDataByNetZeroCommand, NetZeroResponse>
{

    [Authorize(Policy = "User")]
    public async Task<NetZeroResponse> Handle(ModifyPlanDocumentDataByNetZeroCommand request, CancellationToken cancellationToken)
    {
        NetZeroResponse result = new();

        string userId = userService.CurrentNow(cancellationToken).UserId;



        ViewPlanDetail planDetail = await planDetailQuery.GetDtoByIdAsync(request.PlanDetailId, userId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {request.PlanDetailId} not found.");


        Plan plan = await planQuery.GetByIdAsync((long)planDetail.PlanId, cancellationToken) ??
             throw new NotFoundException($"Plan with ID {planDetail.PlanId} not found.");

        ViewCompany company = await tenantQuery.CompanyDtoByIdAsync(plan.CompanyId, cancellationToken) ??
             throw new NotFoundException($"company with ID {plan.CompanyId} not found.");

        QueryApiConnectionRequest apiRequest = new() { ConnectionType = "NetZero", IsEnable = true };
        IEnumerable<ViewApiConnection> apiConnections = await apiConnectionQuery.GetDtoByTenantAsync(apiRequest);

        // 取得設定好的站台識別碼
        string? netZeroToken = apiConnections
        .Where(x => x.ApiConnectionId == planDetail.ApiConnectionId)
        .OrderByDescending(x => x.ModifiedDate)
        .Select(x => x.ConnectionCode)
        .FirstOrDefault();

        if (string.IsNullOrEmpty(netZeroToken))
        {
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, true, cancellationToken);
            result.SetError(NetZeroErrorCode.GetTokenFailed, "Get Token Failed");
            return result;
        }

        using HttpClient client = new();
        HttpClient httpClient = httpClientFactory.CreateClient("NetZero");

        // Step 1: 呼叫 NetZero Api
        HttpRequestMessage getReportListRequest = new(HttpMethod.Get, $"api/em/ghgemissionequivalent/{planDetail.NetzeroReportId}");
        getReportListRequest.Headers.Add("Authorization", "Bearer " + netZeroToken);
        HttpResponseMessage getReportListResponse = await httpClient.SendAsync
            (getReportListRequest);

        if (!getReportListResponse.IsSuccessStatusCode)
        {
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, true, cancellationToken);
            result.SetError(NetZeroErrorCode.GetNetZeroDataFailed, "Get NetZero Data Failed");
            return result;
        }

        // 在response中比對有沒有match的depotId
        string json = await getReportListResponse.Content.ReadAsStringAsync(cancellationToken);

        JsonNode? root = null;
        DateTime startDate;
        DateTime endDate;
        try
        {
            root = JsonNode.Parse(json);
        }
        catch (Exception)
        {
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, true, cancellationToken);
            result.SetError(NetZeroErrorCode.GetNetZeroDataFailed, "Get NetZero Data Failed");
            return result;
        }

        // 按照 scopeCode 分組並計算 deitemGHGequiavlentGWP 總和
        JsonArray scopeLOCEquivalents = root?["scopeLOCEquivalents"]?.AsArray() ?? [];


        // 取得計畫區域資料
        IEnumerable<ViewPlanAreaData> planAreaData = await planFactoryQuery.GetPlanAreaDataByPlanDetailId(request.PlanDetailId,
                                                                                                    userService.CurrentNow(cancellationToken).CurrentTenant.TenantId,
                                                                                                    cancellationToken);

        Dictionary<string, int> areaNameToPlanDocumentId = planDetail.PlanDocumentList
            .Where(pd => pd.PlanFactoryId.HasValue)
            .Join(planAreaData.Where(area => !string.IsNullOrEmpty(area.AreaName)),
                pd => pd.PlanFactoryId.Value,
                area => area.PlanFactoryId,
                (pd, area) => new { area.AreaName, PlanDocumentId = (int)pd.PlanDocumentId })
            .ToDictionary(x => x.AreaName, x => x.PlanDocumentId, StringComparer.OrdinalIgnoreCase);

        // 取得有對應區域的 PlanDocument 清單（用於後續的資料清理）
        List<ViewPlanDocument> planDocumentInArea = [.. planDetail.PlanDocumentList
            .Where(pd => pd.PlanFactoryId.HasValue && areaNameToPlanDocumentId.Values.Contains((int)pd.PlanDocumentId))];

        if (DateTime.TryParse(root?["dataSDate"]?.ToString(), out DateTime sDate) &&
            DateTime.TryParse(root?["dataEDate"]?.ToString(), out DateTime eDate))
        {
            startDate = sDate;
            endDate = eDate;
        }
        else
        {
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, true, cancellationToken);
            result.SetError(NetZeroErrorCode.TryParsetDateFailed, "TryParse StartDate or EndDate Failed");
            return result;
        }



        if (planDocumentInArea != null)
        {

            // TODO 這裡要加入splitConfig判斷
            // 取得splitConfig資料
            //var configs = await planDocumentDataSplitedQuery.GetConfigListDtoAsync(cancellationToken);
            IEnumerable<ViewPlanDocumentDataSplitConfig> configs = await planDocumentDataSplitedQuery.GetConfigListDtoAsync(cancellationToken);

            foreach (ViewPlanDocument item in planDocumentInArea)
            {
                // 移除原本的planDocumentData、planDocumentDataSplited資料
                await planRepository.RemovePlanDocumentDataByPlanDocumentIdAsync((int)item.PlanDocumentId, cancellationToken);
            }


            foreach (JsonNode? scopeLOCEquivalent in scopeLOCEquivalents)
            {
                string? netZeroAreaName = scopeLOCEquivalent?["locName"]?.ToString();

                if (string.IsNullOrEmpty(netZeroAreaName) ||
                    !areaNameToPlanDocumentId.TryGetValue(netZeroAreaName, out int planDocumentId))
                {
                    logger.LogWarning("無法找到對應的區域或 PlanDocument，區域名稱: {AreaName}", netZeroAreaName);
                    continue;
                }

                for (int i = 1; i <= 3; i++)
                {
                    string[] fieldNameList = ["範疇一排放量", "範疇二排放量", "範疇三排放量"];
                    // 新增範疇一 ~ 三 排放量
                    PlanDocumentData planDocumentData = new()
                    {
                        PlanDocumentId = planDocumentId,
                        DocumentId = 0,
                        TenantId = userService.CurrentNow(cancellationToken).CurrentTenant?.TenantId,
                        FieldId = $"Scope{i}Emission",
                        CustomName = $"Scope{i}Emission",
                        FieldType = "number",
                        FieldValue = GetRoundedValue(scopeLOCEquivalent, $"scope{i}"),
                        FieldName = fieldNameList[i - 1],
                        Required = true,
                        Archived = false,
                        CompanyName = company.CompanyName,
                        AreaName = scopeLOCEquivalent?["locName"]?.ToString(),
                        StartDate = startDate,
                        EndDate = endDate,
                    };
                    _ = await planDocumentDataRepository.AddAsync(planDocumentData, cancellationToken);

                    // TODO 這裡要加入splitConfig判斷
                    //var isMatchConfig = configs.Count() > 0 && configs.Select(x => x.FieldID).Any(x => x == planDocumentData.CustomName);
                    bool isMatchConfig = true;
                    List<PlanDocumentDataSplited> splitedData = SplitPlanDocumentDataByMonth(planDocumentData, (int)planDetail.Year, isMatchConfig);
                    await planDocumentDataRepository.AddSplitedDataRangeAsync(splitedData.ToArray(), cancellationToken);
                }
            }
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, false, cancellationToken);
        }
        else
        {
            await planRepository.SetPlanDetailApiConnectionFailedAsync(request.PlanDetailId, true, cancellationToken);
            result.SetError(NetZeroErrorCode.GetPlanDocumentFailed, "Get PlanDocument Failed");
            return result;
        }



        return result;
    }



    ///<summary>
    /// 取得數值資料並轉為小數點以下四位
    ///</summary>
    private static string GetRoundedValue(JsonNode? node, string key)
    {
        if (node?[key]?.ToString() is string rawValue &&
            double.TryParse(rawValue, out double number))
        {
            return Math.Round(number, 4, MidpointRounding.AwayFromZero).ToString("F4");
        }
        else
        {
            throw new Exception("TryParse ScopeEmission to FieldValue Failed");
        }
    }


    private static int[] GetDaysInMonths(int year) => [
        31, // 1月
        DateTime.IsLeapYear(year) ? 29 : 28, // 2月
        31, // 3月
        30, // 4月
        31, // 5月
        30, // 6月
        31, // 7月
        31, // 8月
        30, // 9月
        31, // 10月
        30, // 11月
        31  // 12月
    ];

    ///<summary>
    /// 將 PlanDocumentData 拆分成按月份切分的 12 筆 PlanDocumentDataSplited
    ///</summary>
    /// <param name="planDocumentData">原始資料</param>
    /// <param name="year">年份</param>
    /// <param name="isMachedConfig">對應拆分的config</param>
    /// <returns>12 筆按月份切分的 PlanDocumentDataSplited</returns>
    private static List<PlanDocumentDataSplited> SplitPlanDocumentDataByMonth(PlanDocumentData planDocumentData, int year, bool isMatchConfig)
    {
        List<PlanDocumentDataSplited> splitedDataList = new List<PlanDocumentDataSplited>();

        // 取得該年每個月的天數
        int[] daysInMonth = GetDaysInMonths(year);
        int totalDays = daysInMonth.Sum();

        // 解析原始數值
        if (!decimal.TryParse(planDocumentData.FieldValue, out decimal originalValue))
        {
            throw new Exception($"無法解析 FieldValue: {planDocumentData.FieldValue}");
        }

        if (isMatchConfig)
        {
            // 計算每個月的比例值，並確保加總等於原值
            decimal[] monthlyValues = new decimal[12];
            decimal remainingValue = originalValue;

            // 先計算前11個月的值（四捨五入到小數點4位）
            for (int month = 0; month < 11; month++)
            {
                int daysInCurrentMonth = daysInMonth[month];
                decimal monthlyValue = originalValue * daysInCurrentMonth / totalDays;
                monthlyValues[month] = Math.Round(monthlyValue, 4, MidpointRounding.AwayFromZero);
                remainingValue -= monthlyValues[month];
            }

            // 最後一個月使用剩餘值，確保加總等於原值
            monthlyValues[11] = remainingValue;

            // 為每個月建立一筆 PlanDocumentDataSplited
            for (int month = 1; month <= 12; month++)
            {
                PlanDocumentDataSplited splitedData = new PlanDocumentDataSplited
                {
                    PlanDocumentId = planDocumentData.PlanDocumentId,
                    DocumentId = planDocumentData.DocumentId,
                    CompanyName = planDocumentData.CompanyName,
                    AreaName = planDocumentData.AreaName,
                    StartDate = planDocumentData.StartDate,
                    EndDate = planDocumentData.EndDate,
                    TenantId = planDocumentData.TenantId,
                    Archived = planDocumentData.Archived,
                    FieldId = planDocumentData.FieldId,
                    FieldName = planDocumentData.FieldName,
                    FieldType = planDocumentData.FieldType,
                    SplitValue = monthlyValues[month - 1].ToString("F4"),
                    Required = planDocumentData.Required,
                    ReadOnly = planDocumentData.ReadOnly,
                    CycleNumber = month,
                    CycleType = "month",
                    CustomName = planDocumentData.CustomName,
                    CreatedDate = planDocumentData.CreatedDate,
                    CreatedUser = planDocumentData.CreatedUser,
                    ModifiedDate = planDocumentData.ModifiedDate,
                    ModifiedUser = planDocumentData.ModifiedUser
                };

                splitedDataList.Add(splitedData);
            }
        }
        else
        {
            PlanDocumentDataSplited originData = new PlanDocumentDataSplited
            {
                PlanDocumentId = planDocumentData.PlanDocumentId,
                DocumentId = planDocumentData.DocumentId,
                CompanyName = planDocumentData.CompanyName,
                AreaName = planDocumentData.AreaName,
                StartDate = planDocumentData.StartDate,
                EndDate = planDocumentData.EndDate,
                TenantId = planDocumentData.TenantId,
                Archived = planDocumentData.Archived,
                FieldId = planDocumentData.FieldId,
                FieldName = planDocumentData.FieldName,
                FieldType = planDocumentData.FieldType,
                SplitValue = planDocumentData.FieldValue,
                Required = planDocumentData.Required,
                ReadOnly = planDocumentData.ReadOnly,
                CycleNumber = null,
                CycleType = "year",
                CustomName = planDocumentData.CustomName,
                CreatedDate = planDocumentData.CreatedDate,
                CreatedUser = planDocumentData.CreatedUser,
                ModifiedDate = planDocumentData.ModifiedDate,
                ModifiedUser = planDocumentData.ModifiedUser
            };

            splitedDataList.Add(originData);
        }


        return splitedDataList;
    }
}
