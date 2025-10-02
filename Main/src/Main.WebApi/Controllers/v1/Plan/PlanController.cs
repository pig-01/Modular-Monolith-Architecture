using AutoMapper;
using Base.Domain.Exceptions;
using Base.Domain.Models.NPOI;
using Base.Infrastructure.Interface.Authentication;
using Base.NPOI;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.Model.Pagination;
using Main.Dto.ViewModel.Plan;
using Main.Dto.ViewModel.SystemSetting;
using Main.WebApi.Application.Commands.Bizform.Documents;
using Main.WebApi.Application.Commands.Plans;
using Main.WebApi.Application.Models.NetZero;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.SystemSetting;

namespace Main.WebApi.Controllers.v1.Plan;

public class PlanController(
    IMediator mediator,
    IMapper mapper,
    IPlanQuery planQuery,
    IPlanDocumentQuery planDocumentQuery,
    IPlanFactoryQuery planFactoryQuery,
    IUserService<Scuser> userService,
    IHttpClientFactory httpClientFactory,
    IPlanDocumentDataSplitedQuery planDocumentDataSplitedQuery,
    IPlanDocumentLegacyQuery planDocumentLegacyQuery,
    IApiConnectionQuery apiConnectionQuery,
    ILogger<PlanController> logger,
    IPlanDetailQuery planDetailQuery) : BaseController
{
    /// <summary>
    /// 查詢計畫
    /// </summary>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlan([FromQuery] QueryPlanRequest request)
    {
        Scuser scuser = await userService.Now();
        request.Responsible = scuser.UserId;
        request.TenantID = scuser.CurrentTenant.TenantId;
        PaginationResult<ViewPlan> result = await planQuery.ListDtoAsync(request);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 查詢計畫 By ID
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlanByID([FromRoute] int id)
    {
        Scuser scuser = await userService.Now();
        ViewPlan? result = await planQuery.GetDtoByIdAsync(id, scuser.UserId);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 查詢計畫明細 By ID
    /// </summary>
    /// <param name="planId">計畫ID</param>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("{planId}/PlanDetail")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlanDetailByPlanID([FromRoute] int planId)
    {
        Scuser currentUser = await userService.Now();
        string language = currentUser.CurrentCulture;
        IEnumerable<ViewPlanDetail> result = await planDetailQuery.GetDtoByPlanIdAsync(planId, currentUser.UserId);
        return ActionResultBuilder(mapper.Map<IEnumerable<ViewPlanDetail>>(result, opt => opt.Items["Language"] = language));
    }

    /// <summary>
    /// 新增計畫
    /// </summary>
    /// <param name="command">建立要求單位命令，同時具有新增要求單位和版本的功能</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPost("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> InsertPlan([FromBody] CreatePlanCommand command) =>
        ActionResultBuilder(await mediator.Send(command));

    /// <summary>
    /// 更新計畫
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">成功更新計畫</response>
    /// <response code="400">更新計畫失敗</response>
    /// <returns></returns>
    [HttpPut("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> UpdatePlan([FromBody] ModifyPlanCommand command)
    {
        bool result = await mediator.Send(command);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 隱藏計畫
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">成功隱藏計畫</response>
    /// <response code="400">隱藏計畫失敗</response>
    /// <returns></returns>
    [HttpPut("Hide")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<bool>(Status200OK)]
    [ProducesResponseType<bool>(Status400BadRequest)]
    public async Task<IActionResult> HidePlan([FromBody] HidePlanCommand command)
    {
        await mediator.Send(command);
        return ActionResultBuilder(true);
    }

    /// <summary>
    /// 隱藏多選計畫
    /// </summary>
    /// <param name="planIds"></param>
    /// <response code="200">成功隱藏複數計畫</response>
    /// <response code="400">隱藏複數計畫失敗</response>
    /// <returns></returns>
    [HttpPut("HideMultiplePlan")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<bool>(Status200OK)]
    [ProducesResponseType<bool>(Status400BadRequest)]
    public async Task<IActionResult> HideMultiplePlan([FromBody] int[] planIds)
    {
        await mediator.Send(new HideMultiplePlanCommand(planIds));
        return ActionResultBuilder(true);
    }

    /// <summary>
    /// 刪除計畫
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">成功刪除計畫</response>
    /// <response code="400">刪除計畫失敗</response>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<bool>(Status200OK)]
    [ProducesResponseType<bool>(Status400BadRequest)]
    public async Task<IActionResult> DeletePlan([FromRoute] int id)
    {
        try
        {
            await mediator.Send(new CancelMultiplePlanCommand(id));
            return ActionResultBuilder(true);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 刪除複數計畫
    /// </summary>
    /// <param name="ids"></param>
    /// <response code="200">成功刪除複數計畫</response>
    /// <response code="400">刪除複數計畫失敗</response>
    /// <returns></returns>
    [HttpDelete("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<bool>(Status200OK)]
    [ProducesResponseType<bool>(Status400BadRequest)]
    public async Task<IActionResult> DeletePlans([FromQuery] string ids)
    {
        try
        {
            int[] planIds = [.. ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)];
            await mediator.Send(new CancelMultiplePlanCommand(planIds));
            return ActionResultBuilder(true);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 查詢指標明細
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">成功查詢指標明細</response>
    /// <response code="404">查無查詢指標明細</response>
    /// <returns></returns>
    [HttpGet("PlanDetail")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<IEnumerable<ViewPlanDetail>>(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> PlanDetail([FromQuery] QueryPlanDetailRequest request)
    {
        IEnumerable<ViewPlanDetail> result = await planDetailQuery.ListDtoAsync(request);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 依照ID查詢指標明細
    /// </summary>
    /// <param name="id">指標明細ID</param>
    /// <response code="200">成功查詢指標明細</response>
    /// <response code="400">查詢指標明細失敗</response>
    /// <response code="404">查無查詢指標明細</response>
    /// <returns></returns>
    [HttpGet("PlanDetail/{id}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<ViewPlanDetail>(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> PlanDetail([FromRoute] int id)
    {
        Scuser currentUser = await userService.Now();
        string language = currentUser.CurrentCulture;
        return ActionResultBuilder(mapper.Map<ViewPlanDetail>(await planDetailQuery.GetDtoByIdAsync(id, currentUser.UserId), opt => opt.Items["Language"] = language));
    }

    /// <summary>
    /// 依照ID查詢指標明細
    /// </summary>
    /// <response code="200">成功查詢指標明細</response>
    /// <response code="400">查詢指標明細失敗</response>
    /// <response code="404">查無查詢指標明細</response>
    /// <returns></returns>
    [HttpGet("PlanDetail/{planDetailId}/Legacy")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<ViewPlanDocumentLegacy>(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> PlanDocumentLegacy([FromRoute] int planDetailId) => ActionResultBuilder(await planDocumentLegacyQuery.GetDtoByDetailIdAsync(planDetailId));


    /// <summary>
    /// 指派指標表單指派人
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="request">指派指派人命令</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPut("PlanDetail/Assign")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> AssignPlanDetail([FromBody] AssignPlanDetailCommand request)
    {
        try
        {
            // if assign plan detail failed, return false
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 指派指標明細表單指派人
    /// </summary>
    /// <remarks>
    /// 建立指標明細表單資料
    /// </remarks>
    /// <param name="request">指派指派人命令</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPut("PlanDocument/Assign")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> AssignPlanDocument([FromBody] AssignPlanDocumentCommand request)
    {
        try
        {
            // if assign plan detail failed, return false
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 指標填寫通知
    /// </summary>
    /// <param name="request">通知param</param>
    /// <response code="200">成功通知</response>
    /// <response code="400">通知失敗</response>
    /// <returns></returns>
    [HttpPut("PlanDocument/Notify")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> NotifyPlanDocument([FromBody] NotifyPlanDocumentCommand request)
    {
        try
        {
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 附加指標明細表單Bizform表單ID
    /// </summary>
    /// <remarks>
    /// 寫入指標明細表單Bizform表單ID
    /// </remarks>
    /// <param name="request">附加表單ID命令</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPut("PlanDocument/Attach")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> AttachPlanDocument([FromBody] AttachDocument2PlanDecumentCommand request)
    {
        try
        {
            // if assign plan detail failed, return false
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 修改計畫明細週期
    /// </summary>
    /// <param name="command">修改計畫明細週期命令</param>
    /// <returns></returns>
    [HttpPut("PlanDetail/Cycle")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ModifyPlanDetailCycle([FromBody] ModifyPlanDetailCycleCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    [HttpPut("PlanDocument/Archive")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ArchivePlanDocument([FromBody] ArchivePlanDocumentCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    /// <summary>
    /// 同步計畫文件狀態
    /// </summary>
    /// <param name="planDocumentId"></param>
    /// <returns></returns>
    [HttpPut("PlanDocument/{planDocumentId}/Status")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> SyncPlanDocumentStatus([FromRoute] int planDocumentId)
    {
        logger.LogInformation("SyncPlanDocumentStatus planDocumentId: {planDocumentId}", planDocumentId);
        return ActionResultBuilder(await mediator.Send(new SyncDocumentStatusCommand(planDocumentId)));
    }

    /// <summary>
    /// 匯出計畫
    /// </summary>
    /// <param name="planDetailIdList"></param>
    /// <response code="200">匯出成功</response>
    /// <response code="400">匯出失敗</response>
    /// <returns></returns>
    [HttpGet("ExportPlan")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ExportPlan([FromQuery] string planDetailIdList, string indicatorIdList)
    {
        string[] idList = indicatorIdList?.Split(",") ?? [];
        string relativePath = "Templates\\Excel"; // 專案相對路徑下的目錄
        string fileName = "企業Demo資訊揭露指標Template.xlsx"; // 要使用的範本文件名
        Dictionary<string, string> resultFiles = [];
        if (idList.Contains("DataSet") && idList.Length > 1 || !idList.Contains("DataSet") && idList.Length > 0)
        {
            // 組合完整路徑
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath, fileName);

            using (NpoiUtil npoiUtil = NpoiUtil.WorkFactory(fullPath))
            {

                string fieldList = string.Join(",", npoiUtil.GetReplaceParamListFromExcel());
                Dictionary<string, object> modelData = await planDocumentQuery.GetPlanDocumentDataForExcel(planDetailIdList, fieldList);

                npoiUtil.ReplaceDataInExcel(modelData);
                using MemoryStream exportStream = new();
                npoiUtil.ExportMemoryStream(exportStream);
                string base64File = Convert.ToBase64String(exportStream.ToArray());
                // TODO 先分兩種匯出格式，一種是Dataset，一種是Template
                // 如果之後有需要BY 指標有不同的EXCEL，再來修改
                resultFiles.Add("Template", base64File);
            }
        }


        if (idList.Contains("DataSet"))
        {
            var rawList = await planQuery.GetExportDataSetAsync(planDetailIdList?.Split(",") ?? []);

            var dataSets = new Dictionary<string, List<Dictionary<string, object>>>();

            // group by PlanDetailName
            foreach (var groupByPlan in rawList.GroupBy(x => x.PlanDetailName))
            {
                var planDetailList = new List<Dictionary<string, object>>();

                // 再根據 CycleType 做群組
                var cycleGroups = groupByPlan
                    .GroupBy(x =>
                    {
                        return x.CycleType switch
                        {
                            "year" => x.Year?.ToString() ?? "",
                            "quarter" => x.Quarter?.ToString() ?? "",
                            "month" => x.Month?.ToString() ?? "",
                            _ => ""
                        };
                    });

                foreach (var cycleGroup in cycleGroups)
                {
                    var dict = new Dictionary<string, object>();

                    // 將FieldName作為欄位加入
                    foreach (var item in cycleGroup)
                    {
                        if (!string.IsNullOrEmpty(item.FieldName))
                        {
                            if (!dict.ContainsKey(item.FieldName))
                            {
                                dict[item.FieldName] = item.FieldValue;
                            }
                        }
                        else if (!string.IsNullOrEmpty(item.FieldID))
                        {
                            if (!dict.ContainsKey(item.FieldID))
                            {
                                dict[item.FieldID] = item.FieldValue;
                            }
                        }
                    }

                    planDetailList.Add(dict);
                }

                dataSets[groupByPlan.Key] = planDetailList;
            }
            using (NpoiUtil npoiUtil = NpoiUtil.WorkFactory(NpoiUtil.ExcelType.xlsx))
            {

                while (npoiUtil.WorkBook.NumberOfSheets < dataSets.Count)
                {
                    npoiUtil.WorkBook.CreateSheet(); // 先用預設名稱建立
                }
                int sheetIndex = 0;
                foreach (var dataSet in dataSets)
                {
                    string sheetName = dataSet.Key;
                    npoiUtil.WorkBook.SetSheetName(sheetIndex, sheetName.Length > 30 ? sheetName[..30] : sheetName);

                    var excelFields = dataSet.Value[0].Keys.ToList().Select(fieldName => new NPOIDataField()
                    {
                        Field = fieldName,
                        Name = fieldName,
                        Width = 40,
                    }).ToList();

                    npoiUtil.SetExcelDataExportPosition(0, 1, 0, sheetIndex);
                    npoiUtil.InitDataFields(excelFields);
                    npoiUtil.ExportDataToExcel(dataSet.Value);
                    sheetIndex++;
                }

                using (MemoryStream exportStream = new())
                {
                    npoiUtil.ExportMemoryStream(exportStream);
                    string base64File = Convert.ToBase64String(exportStream.ToArray());
                    // TODO 先分兩種匯出格式，一種是Dataset，一種是Template
                    // 如果之後有需要BY 指標有不同的EXCEL，再來修改
                    resultFiles.Add("DataSet", base64File);
                }
            }
        }

        return Ok(resultFiles);
    }


    [HttpGet("NetZero/ReportList")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> GetNetZeroReportList([FromQuery] int apiConnectionId, int year)
    {
        try
        {

            QueryApiConnectionRequest apiRequest = new() { ConnectionType = "NetZero", IsEnable = true };
            IEnumerable<ViewApiConnection> apiConnections = await apiConnectionQuery.GetDtoByTenantAsync(apiRequest);

            // 取得設定好的站台識別碼
            string? netZeroToken = apiConnections
            .Where(x => x.ApiConnectionId == apiConnectionId)
            .OrderByDescending(x => x.ModifiedDate)
            .Select(x => x.ConnectionCode)
            .FirstOrDefault();

            if (string.IsNullOrEmpty(netZeroToken))
            {
                throw new Exception("Get Token From ApiConnection Failed");
            }

            using HttpClient client = new();
            HttpClient httpClient = httpClientFactory.CreateClient("NetZero");

            // Step 1: 呼叫 NetZero Api
            HttpRequestMessage getReportListRequest = new(HttpMethod.Get, $"api/em/ghgemissionlist/{year}");
            getReportListRequest.Headers.Add("Authorization", "Bearer " + netZeroToken);
            HttpResponseMessage getReportListResponse = await httpClient.SendAsync(getReportListRequest);


            if (getReportListResponse.IsSuccessStatusCode)
            {
                string responseBody = await getReportListResponse.Content.ReadAsStringAsync();
                return Content(responseBody, "application/json");
            }
            else
            {
                logger.LogError("Get Report List Failed, StatusCode: {StatusCode}, Response: {Response}", getReportListResponse.StatusCode, await getReportListResponse.Content.ReadAsStringAsync());
                return BadRequest("Get Report List Failed, StatusCode: " + getReportListResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get Report List Failed, Exception: {Exception}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("PlanDocumentDataSplited/{planDocumentId}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> GetPlanDocumentDataSplitedByPlanDocumentId([FromRoute] int planDocumentId) =>
        Ok(await planDocumentDataSplitedQuery.GetDtoByPlanDocumentIdAsync(planDocumentId));

    [HttpPost("NetZero/Connect")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ConnectPlanDetailToNetZero([FromBody] ConnectNetZeroToPlanDetailCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    [HttpPost("NetZero/CancelConnect")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> CancelConnectPlanDetailToNetZero([FromBody] CancelConnectNetZeroToPlanDetailCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    [HttpPost("NetZero/ModifyPlanDocumentData")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ModifyPlanDocumentDataByNetZero([FromBody] ModifyPlanDocumentDataByNetZeroCommand command)
    {
        try
        {
            NetZeroResponse result = await mediator.Send(command);

            // 直接回傳 NetZeroResponse，無論成功或失敗都是 HTTP 200
            return Ok(result);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("PlanDetail/HideHint")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> HidePlanDetailHint([FromBody] HidePlanDetailHintCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }

    [HttpGet("PlanDocument/{planId}/AreaList")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> GetPlanAreaData([FromRoute] int planId, CancellationToken cancellationToken = default)
    {
        Scuser scuser = await userService.Now(cancellationToken);
        IEnumerable<ViewPlanAreaData> result = await planFactoryQuery.GetPlanAreaDataByPlanId(planId, scuser.CurrentTenant.TenantId, cancellationToken);
        return ActionResultBuilder(result);
    }

    [HttpGet("PlanDocument/{planDocumentId}/IsSyncing")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> CheckPlanDocumentAsync([FromRoute] int planDocumentId)
    {
        try
        {
            CheckPlanDocumentResult result = await mediator.Send(new CheckPlanDocumentCommand(planDocumentId));
            return Ok(result);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
