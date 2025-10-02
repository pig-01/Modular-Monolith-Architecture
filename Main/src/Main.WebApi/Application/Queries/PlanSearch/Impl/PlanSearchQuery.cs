using System.Collections;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Utilities.SqlBuilder;
using Main.Domain.AggregatesModel.PlanSearchAggreate;
using Main.Dto.ViewModel.PlanSearch;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.PlanSearch.Impl;


public class PlanSearchQuery(DemoContext context,
 ITimeZoneService timeZoneService) : BaseQuery, IPlanSearchQuery
{
    // private readonly DemoContext context = context;
    // private readonly ITimeZoneService timeZoneService = timeZoneService;

    /// <summary>
    /// 搜尋類型枚舉
    /// </summary>
    private enum SearchType
    {
        PlanLevel,
        DetailLevel,
        FieldLevel
    }

    /// <summary>
    /// 搜尋結果層級枚舉
    /// </summary>
    private enum DisplayLevel
    {
        PlanOnly,       // 只顯示計畫基本資訊
        PlanWithDetail, // 顯示計畫+指標資訊
        FullHierarchy   // 完整階層資訊
    }

    /// <summary>
    /// 智慧偵測搜尋類型
    /// </summary>
    /// <param name="keyword">搜尋關鍵字</param>
    /// <param name="matchedData">匹配的資料</param>
    /// <returns>搜尋類型和顯示層級</returns>
    private static (SearchType searchType, DisplayLevel displayLevel) DetectSearchType(string keyword, List<ViewCrossPlanSearch> matchedData)
    {
        if (!matchedData.Any())
            return (SearchType.PlanLevel, DisplayLevel.PlanOnly);

        // 統計各層級的匹配數量
        int planLevelMatches = 0;
        int detailLevelMatches = 0;
        int fieldLevelMatches = 0;

        // 去重用的集合
        HashSet<string> processedPlanNames = [];
        HashSet<(string planName, string detailName)> processedDetailCombinations = [];

        foreach (ViewCrossPlanSearch item in matchedData)
        {
            // 計畫層級匹配檢查 - 先檢查是否已處理過
            if (!processedPlanNames.Contains(item.PlanName))
            {
                if (IsFieldMatch(keyword, item.PlanName) ||
                    IsFieldMatch(keyword, item.CompanyName) ||
                    IsFieldMatch(keyword, item.CompanyCode) ||
                    IsFieldMatch(keyword, item.AreaName) ||
                    IsFieldMatch(keyword, item.AreaCode))
                {
                    planLevelMatches++;
                    processedPlanNames.Add(item.PlanName);
                }
            }

            // 指標層級匹配檢查 - 檢查 (PlanName, PlanDetailName) 組合
            (string PlanName, string? PlanDetailName) detailCombination = (item.PlanName, item.PlanDetailName);
            if (!processedDetailCombinations.Contains(detailCombination))
            {
                if (IsFieldMatch(keyword, item.PlanDetailName) ||
                    IsFieldMatch(keyword, item.PlanDetailChName) ||
                    IsFieldMatch(keyword, item.PlanDetailEnName) ||
                    IsFieldMatch(keyword, item.PlanDetailJpName) ||
                    IsFieldMatch(keyword, item.RowIdNumber))
                {
                    detailLevelMatches++;
                    processedDetailCombinations.Add(detailCombination);
                }
            }

            // 欄位層級匹配檢查
            if (IsFieldMatch(keyword, item.FieldName) ||
                IsFieldMatch(keyword, item.FieldValue) ||
                IsFieldMatch(keyword, item.CustomName) ||
                IsFieldMatch(keyword, item.DataAreaName))
            {
                fieldLevelMatches++;
            }
        }

        // 比例分析邏輯
        int totalMatches = planLevelMatches + detailLevelMatches + fieldLevelMatches;
        if (totalMatches == 0)
            return (SearchType.PlanLevel, DisplayLevel.PlanOnly);

        double planRatio = (double)planLevelMatches / totalMatches;
        double detailRatio = (double)detailLevelMatches / totalMatches;
        double fieldRatio = (double)fieldLevelMatches / totalMatches;

        // 比例閾值判斷 (60% 規則)
        const double dominantThreshold = 0.6;

        if (fieldRatio > dominantThreshold)
        {
            return (SearchType.FieldLevel, DisplayLevel.FullHierarchy);
        }
        else if (detailRatio > dominantThreshold)
        {
            return (SearchType.DetailLevel, DisplayLevel.PlanWithDetail);
        }
        else if (planRatio > dominantThreshold)
        {
            return (SearchType.PlanLevel, DisplayLevel.PlanOnly);
        }
        else
        {
            // 沒有明顯主導，選擇數量最多的層級
            return GetHighestCountType(planLevelMatches, detailLevelMatches, fieldLevelMatches);
        }
    }

    /// <summary>
    /// 檢查欄位是否匹配關鍵字
    /// </summary>
    private static bool IsFieldMatch(string keyword, string? fieldValue)
    {
        return !string.IsNullOrEmpty(fieldValue) &&
               fieldValue.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 依據關鍵字搜尋計畫
    /// </summary>
    /// <param name="request">搜尋請求</param>
    /// <returns>搜尋結果</returns>
    /// <remarks>
    /// 階層式搜尋邏輯：
    /// 1. 智慧偵測搜尋類型（計畫層級/指標層級/欄位層級）
    /// 2. 根據搜尋類型決定資料展示深度
    /// 3. 整合 PlanTemplate 資料補填未填寫的指標
    /// 4. 建構對應層級的樹狀結構
    /// </remarks>
    public async Task<List<ViewPlanSearchTreeData>> SearchPlansAsync(QueryPlanSearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.KeyWord))
                return [];

            string keyword = request.KeyWord.Trim();

            // 階段1：執行初始搜尋
            List<ViewCrossPlanSearch> initialResults = await ExecuteInitialSearch(keyword, request);

            if (initialResults.Count == 0)
                return [];

            // 階段2：智慧偵測搜尋類型
            (SearchType searchType, DisplayLevel displayLevel) = DetectSearchType(keyword, initialResults);

            // 階段3：根據搜尋類型進行階層式資料過濾
            List<ViewCrossPlanSearch> filteredData = await ApplyHierarchicalFiltering(
                initialResults, searchType, displayLevel, keyword, request);

            // 階段4：整合 PlanTemplate 資料（如需要）目前暫時不用
            if (searchType != SearchType.FieldLevel)
            {
                filteredData = await IntegratePlanTemplateData(filteredData, request);
            }

            // 階段5：建構對應層級的樹狀結構
            return BuildTreeStructureByLevel(filteredData, displayLevel);
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    /// <summary>
    /// 執行初始搜尋
    /// </summary>
    private async Task<List<ViewCrossPlanSearch>> ExecuteInitialSearch(string keyword, QueryPlanSearchRequest request)
    {
        string searchSql = @"
            SELECT
                v.PlanID,
                v.PlanName,
                v.PlanYear,
                v.CompanyID,
                v.CompanyName,
                v.CompanyCode,
                v.AreaID,
                v.AreaName,
                v.AreaCode,
                v.PlanDetailID,
                v.PlanDetailName,
                v.PlanDetailChName,
                v.PlanDetailEnName,
                v.PlanDetailJpName,
                v.CycleType,
                v.CycleNumber,
                v.GroupID,
                v.RowNumber,
                CONCAT(v.GroupID COLLATE Chinese_Taiwan_Stroke_CI_AS, '-', v.RowNumber COLLATE Chinese_Taiwan_Stroke_CI_AS) AS RowIdNumber,
                v.PlanDocumentDataID,
                v.FieldName,
                v.FieldValue,
                v.FieldType,
                v.Unit,
                v.CustomName,
                v.DataAreaName
            FROM vwCrossPlanSearch v
            INNER JOIN dbo.fnGetPlanPermissions(@responsible) p
                ON v.PlanID = p.PlanID
            WHERE (
                v.PlanName LIKE @keyword
                OR v.CompanyName LIKE @keyword
                OR v.CompanyCode LIKE @keyword
                OR v.AreaName LIKE @keyword
                OR v.AreaCode LIKE @keyword
                OR v.PlanDetailName LIKE @keyword
                OR v.PlanDetailChName LIKE @keyword
                OR v.PlanDetailEnName LIKE @keyword
                OR v.PlanDetailJpName LIKE @keyword
                OR v.FieldName LIKE @keyword
                OR v.FieldValue LIKE @keyword
                OR v.CustomName LIKE @keyword
                OR v.DataAreaName LIKE @keyword
                OR CONCAT(v.GroupID COLLATE Chinese_Taiwan_Stroke_CI_AS, '-', v.RowNumber COLLATE Chinese_Taiwan_Stroke_CI_AS) LIKE @keyword
            )
            AND v.TenantID = @tenantId
            AND p.HasViewPermission = 1
            AND (@startYear IS NULL OR v.PlanYear >= @startYear)
            AND (@endYear IS NULL OR v.PlanYear <= @endYear)
            AND (@companyId IS NULL OR v.CompanyID = @companyId)
            AND (@areaCode IS NULL OR v.AreaCode = @areaCode)
            ORDER BY v.PlanID, v.PlanDetailID, v.PlanDocumentDataID";

        IEnumerable<ViewCrossPlanSearch> results = await context.QueryAsync<ViewCrossPlanSearch>(
            searchSql,
            new
            {
                keyword = $"%{keyword}%",
                tenantId = request.TenantID,
                responsible = request.Responsible,
                startYear = request.StartYear == 0 ? null : request.StartYear,
                endYear = request.EndYear == 0 ? null : request.EndYear,
                companyId = request.CompanyId,
                areaCode = request.AreaCode
            }
        );

        return [.. results];
    }

    /// <summary>
    /// 根據搜尋類型執行階層式資料過濾
    /// </summary>
    private async Task<List<ViewCrossPlanSearch>> ApplyHierarchicalFiltering(
        List<ViewCrossPlanSearch> initialData,
        SearchType searchType,
        DisplayLevel displayLevel,
        string keyword,
        QueryPlanSearchRequest request)
    {
        switch (searchType)
        {
            case SearchType.PlanLevel:
                return await FilterForPlanLevel(initialData, keyword, request);

            case SearchType.DetailLevel:
                return await FilterForDetailLevel(initialData, keyword, request);

            case SearchType.FieldLevel:
                return initialData; // 欄位層級搜尋保留完整資料

            default:
                return initialData;
        }
    }

    /// <summary>
    /// 計畫層級過濾：補充該計畫的完整 PlanDetail 但不包含 Document 資料
    /// </summary>
    private async Task<List<ViewCrossPlanSearch>> FilterForPlanLevel(
        List<ViewCrossPlanSearch> initialData,
        string keyword,
        QueryPlanSearchRequest request)
    {
        List<int> planLevelMatchedPlanIds = [.. initialData
            .Where(item => IsFieldMatch(keyword, item.PlanName) ||
                        IsFieldMatch(keyword, item.CompanyName) ||
                        IsFieldMatch(keyword, item.CompanyCode) ||
                        IsFieldMatch(keyword, item.AreaName) ||
                        IsFieldMatch(keyword, item.AreaCode))
            .Select(item => item.PlanID)
            .Distinct()];

        if (planLevelMatchedPlanIds.Count == 0)
            return initialData;

        // 獲取這些計畫的基本資料和 PlanDetail，但排除 Document 層級資料
        string planLevelSql = @"
            SELECT DISTINCT
                v.PlanID,
                v.PlanName,
                v.PlanYear,
                v.CompanyID,
                v.CompanyName,
                v.CompanyCode,
                v.AreaID,
                v.AreaName,
                v.AreaCode,
                v.PlanDetailID,
                v.PlanDetailName,
                v.PlanDetailChName,
                v.PlanDetailEnName,
                v.PlanDetailJpName,
                v.CycleType,
                v.CycleNumber,
                v.GroupID,
                v.RowNumber,
                CONCAT(v.GroupID COLLATE Chinese_Taiwan_Stroke_CI_AS, '-', v.RowNumber COLLATE Chinese_Taiwan_Stroke_CI_AS) AS RowIdNumber,
                NULL as PlanDocumentDataID,
                NULL as FieldName,
                NULL as FieldValue,
                NULL as FieldType,
                NULL as Unit,
                NULL as CustomName,
                NULL as DataAreaName
            FROM vwCrossPlanSearch v
            INNER JOIN dbo.fnGetPlanPermissions(@responsible) p
                ON v.PlanID = p.PlanID
            WHERE v.PlanID IN @planIds
            AND v.TenantID = @tenantId
            AND p.HasViewPermission = 1
            ORDER BY v.PlanID, v.PlanDetailID";

        IEnumerable<ViewCrossPlanSearch> planLevelData = await context.QueryAsync<ViewCrossPlanSearch>(
            planLevelSql,
            new { planIds = planLevelMatchedPlanIds, tenantId = request.TenantID, responsible = request.Responsible }
        );

        return [.. planLevelData];
    }

    /// <summary>
    /// 指標層級過濾：保留 Plan 和 PlanDetail 資料，但限制 Document 資料
    /// </summary>
    private async Task<List<ViewCrossPlanSearch>> FilterForDetailLevel(
        List<ViewCrossPlanSearch> initialData,
        string keyword,
        QueryPlanSearchRequest request)
    {
        List<ViewCrossPlanSearch> detailMatchedItems = [.. initialData
            .Where(item => IsFieldMatch(keyword, item.PlanDetailName) ||
                        IsFieldMatch(keyword, item.PlanDetailChName) ||
                        IsFieldMatch(keyword, item.PlanDetailEnName) ||
                        IsFieldMatch(keyword, item.PlanDetailJpName) ||
                        IsFieldMatch(keyword, item.RowIdNumber))];

        // 如果有指標層級匹配，獲取相關的完整計畫資料但限制 Document 展示
        List<int> matchedPlanIds = [.. detailMatchedItems.Select(x => x.PlanID).Distinct()];
        List<int?> matchedDetailIds = [.. detailMatchedItems.Select(x => x.PlanDetailID).Where(x => x.HasValue).Distinct()];

        if (!matchedPlanIds.Any())
            return initialData;

        string detailLevelSql = @"
            SELECT
                v.PlanID,
                v.PlanName,
                v.PlanYear,
                v.CompanyID,
                v.CompanyName,
                v.CompanyCode,
                v.AreaID,
                v.AreaName,
                v.AreaCode,
                v.PlanDetailID,
                v.PlanDetailName,
                v.PlanDetailChName,
                v.PlanDetailEnName,
                v.PlanDetailJpName,
                v.CycleType,
                v.CycleNumber,
                v.GroupID,
                v.RowNumber,
                CONCAT(v.GroupID COLLATE Chinese_Taiwan_Stroke_CI_AS, '-', v.RowNumber COLLATE Chinese_Taiwan_Stroke_CI_AS) AS RowIdNumber,
                v.PlanDocumentDataID,
                v.FieldName,
                v.FieldValue,
                v.FieldType,
                v.Unit,
                v.CustomName,
                v.DataAreaName
            FROM vwCrossPlanSearch v
            INNER JOIN dbo.fnGetPlanPermissions(@responsible) p
                ON v.PlanID = p.PlanID
            WHERE v.PlanDetailID IN @detailIds
            AND v.TenantID = @tenantId
            AND p.HasViewPermission = 1
            ORDER BY v.PlanID, v.PlanDetailID, v.PlanDocumentDataID";

        IEnumerable<ViewCrossPlanSearch> detailLevelData = await context.QueryAsync<ViewCrossPlanSearch>(
            detailLevelSql,
            new
            {
                detailIds = matchedDetailIds,
                tenantId = request.TenantID,
                responsible = request.Responsible
            }
        );

        return [.. detailLevelData];
    }

    /// <summary>
    /// 整合 PlanTemplate 範本資料補填功能
    /// </summary>
    private async Task<List<ViewCrossPlanSearch>> IntegratePlanTemplateData(
        List<ViewCrossPlanSearch> existingData,
        QueryPlanSearchRequest request)
    {
        try
        {
            // 獲取已有資料的計畫ID列表
            List<int> existingPlanIds = [.. existingData.Select(x => x.PlanID).Distinct()];
            if (!existingPlanIds.Any())
                return existingData;

            // 查詢這些計畫應該有的完整 PlanTemplate 結構
            string templateSql = @"
                SELECT
                    p.PlanID,
                    p.PlanName,
                    p.Year as PlanYear,
                    p.CompanyID,
                    ce.CompanyName,
                    ce.CompanyCode,
                    a.AreaID,
                    a.AreaName,
                    a.AreaCode,
                    pt.PlanTemplateId as PlanDetailID,
                    pt.PlanTemplateName as PlanDetailName,
                    pt.PlanTemplateChName as PlanDetailChName,
                    pt.PlanTemplateEnName as PlanDetailEnName,
                    pt.PlanTemplateJpName as PlanDetailJpName,
                    pt.CycleType,
                    NULL as CycleNumber,
                    pt.GroupId,
                    pt.RowNumber,
                    CONCAT(pt.GroupId COLLATE Chinese_Taiwan_Stroke_CI_AS, '-', pt.RowNumber COLLATE Chinese_Taiwan_Stroke_CI_AS) AS RowIdNumber,
                    NULL as PlanDocumentDataID,
                    NULL as FieldName,
                    NULL as FieldValue,
                    NULL as FieldType,
                    NULL as Unit,
                    NULL as CustomName,
                    NULL as DataAreaName
                FROM [Plan] p
                JOIN CompanyEvent ce ON p.CompanyID = ce.CompanyID
                JOIN PlanFactory pf ON p.PlanID = pf.PlanID
                JOIN Area a ON pf.FactoryID = a.AreaCode AND p.CompanyID = a.CompanyID
                JOIN PlanTemplate pt ON pt.Version = p.PlanTemplateVersion
                LEFT JOIN PlanDetail pd ON p.PlanID = pd.PlanID AND pt.PlanTemplateId = pd.PlanTemplateId
                INNER JOIN dbo.fnGetPlanPermissions(@responsible) perm ON p.PlanID = perm.PlanID
                WHERE p.PlanID IN @planIds
                AND p.TenantID = @tenantId
                AND p.Show = 1
                AND p.Archived = 0
                AND pt.IsDeploy = 1
                AND perm.HasViewPermission = 1
                AND pd.PlanDetailId IS NULL  -- 只取未建立的 PlanDetail
                ORDER BY p.PlanID, pt.SortSequence";

            IEnumerable<ViewCrossPlanSearch> templateData = await context.QueryAsync<ViewCrossPlanSearch>(
                templateSql,
                new
                {
                    planIds = existingPlanIds,
                    tenantId = request.TenantID,
                    responsible = request.Responsible
                }
            );

            // 合併現有資料和範本資料
            List<ViewCrossPlanSearch> mergedData = [.. existingData, .. templateData];

            return [.. mergedData.OrderBy(x => x.PlanID)
                        .ThenBy(x => x.PlanDetailID)
                        .ThenBy(x => x.PlanDocumentDataID)];
        }
        catch (Exception)
        {
            // 如果範本整合失敗，返回原始資料
            return existingData;
        }
    }

    /// <summary>
    /// 根據顯示層級建構樹狀結構
    /// </summary>
    /// <param name="data">原始資料</param>
    /// <param name="displayLevel">顯示層級</param>
    /// <returns>對應層級的樹狀結構</returns>
    private List<ViewPlanSearchTreeData> BuildTreeStructureByLevel(List<ViewCrossPlanSearch> data, DisplayLevel displayLevel)
    {
        switch (displayLevel)
        {
            case DisplayLevel.PlanOnly:
                return BuildPlanOnlyStructure(data);

            case DisplayLevel.PlanWithDetail:
                return BuildPlanWithDetailStructure(data);

            case DisplayLevel.FullHierarchy:
                return BuildFullHierarchyStructure(data, timeZoneService);

            default:
                return BuildFullHierarchyStructure(data, timeZoneService);
        }
    }

    /// <summary>
    /// 建構僅計畫層級的結構（搜尋計畫名稱時使用）
    /// </summary>
    private static List<ViewPlanSearchTreeData> BuildPlanOnlyStructure(List<ViewCrossPlanSearch> data)
    {
        List<ViewPlanSearchTreeData> result = [];

        IEnumerable<IGrouping<int, ViewCrossPlanSearch>> planGroups = data.GroupBy(x => x.PlanID);

        foreach (IGrouping<int, ViewCrossPlanSearch> planGroup in planGroups)
        {
            ViewCrossPlanSearch firstRow = planGroup.First();

            ViewPlanSearchTreeData planData = new()
            {
                PlanId = firstRow.PlanID,
                PlanName = firstRow.PlanName ?? string.Empty,
                PlanYear = firstRow.PlanYear,
                CompanyName = firstRow.CompanyName ?? string.Empty,
                PlanAreaList = [.. planGroup
                    .Select(x => x.AreaName)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()],
                Details = [] // 計畫層級搜尋不顯示 Details
            };

            result.Add(planData);
        }

        return result;
    }

    /// <summary>
    /// 建構計畫+指標層級的結構（搜尋指標名稱時使用）
    /// </summary>
    private static List<ViewPlanSearchTreeData> BuildPlanWithDetailStructure(List<ViewCrossPlanSearch> data)
    {
        List<ViewPlanSearchTreeData> result = [];

        IEnumerable<IGrouping<int, ViewCrossPlanSearch>> planGroups = data.GroupBy(x => x.PlanID);

        foreach (IGrouping<int, ViewCrossPlanSearch> planGroup in planGroups)
        {
            ViewCrossPlanSearch firstRow = planGroup.First();

            ViewPlanSearchTreeData planData = new()
            {
                PlanId = firstRow.PlanID,
                PlanName = firstRow.PlanName ?? string.Empty,
                PlanYear = firstRow.PlanYear,
                CompanyName = firstRow.CompanyName ?? string.Empty,
                PlanAreaList = [.. planGroup
                    .Select(x => x.AreaName)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()],
                Details = []
            };

            // 添加 PlanDetail 但不包含 Documents
            IEnumerable<IGrouping<int?, ViewCrossPlanSearch>> detailGroups = planGroup
                .Where(x => x.PlanDetailID.HasValue)
                .GroupBy(x => x.PlanDetailID);

            foreach (IGrouping<int?, ViewCrossPlanSearch> detailGroup in detailGroups)
            {
                ViewCrossPlanSearch detailFirst = detailGroup.First();

                ViewPlanDetailData detailData = new()
                {
                    PlanDetailId = detailFirst.PlanDetailID,
                    PlanDetailName = detailFirst.PlanDetailName ?? string.Empty,
                    CycleType = detailFirst.CycleType ?? string.Empty,
                    CycleNumber = detailFirst.CycleNumber ?? 0,
                    RowIdNumber = detailFirst.RowIdNumber ?? string.Empty,
                    Documents = [] // 指標層級搜尋不顯示 Documents
                };

                planData.Details.Add(detailData);
            }

            result.Add(planData);
        }

        return result;
    }

    /// <summary>
    /// 建構完整階層結構（搜尋欄位內容時使用）
    /// </summary>
    private static List<ViewPlanSearchTreeData> BuildFullHierarchyStructure(List<ViewCrossPlanSearch> data, ITimeZoneService timeZoneService)
    {
        List<ViewPlanSearchTreeData> result = [];

        // 按 PlanID 分組
        IEnumerable<IGrouping<int, ViewCrossPlanSearch>> planGroups = data.GroupBy(x => x.PlanID);

        foreach (IGrouping<int, ViewCrossPlanSearch> planGroup in planGroups)
        {
            ViewCrossPlanSearch firstRow = planGroup.First();

            ViewPlanSearchTreeData planData = new()
            {
                PlanId = firstRow.PlanID,
                PlanName = firstRow.PlanName ?? string.Empty,
                PlanYear = firstRow.PlanYear,
                CompanyName = firstRow.CompanyName ?? string.Empty,
                // 收集所有不重複的區域
                PlanAreaList = [.. planGroup
                    .Select(x => x.AreaName)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()],
                Details = []
            };

            // 按 PlanDetailID 分組
            IEnumerable<IGrouping<int?, ViewCrossPlanSearch>> detailGroups = planGroup
                .Where(x => x.PlanDetailID.HasValue)
                .GroupBy(x => x.PlanDetailID);

            foreach (IGrouping<int?, ViewCrossPlanSearch> detailGroup in detailGroups)
            {
                ViewCrossPlanSearch detailFirst = detailGroup.First();

                ViewPlanDetailData detailData = new()
                {
                    PlanDetailId = detailFirst.PlanDetailID,
                    PlanDetailName = detailFirst.PlanDetailName ?? string.Empty,
                    CycleType = detailFirst.CycleType ?? string.Empty,
                    CycleNumber = detailFirst.CycleNumber ?? 0,
                    RowIdNumber = detailFirst.RowIdNumber ?? string.Empty,
                    Documents = []
                };

                // 收集所有文件資料（去重）
                IEnumerable<IGrouping<int?, ViewCrossPlanSearch>> documentGroups = detailGroup
                    .Where(x => x.PlanDocumentDataID.HasValue)
                    .GroupBy(x => x.PlanDocumentDataID);
                //

                if (!documentGroups.Any())
                {
                    continue;
                }

                foreach (IGrouping<int?, ViewCrossPlanSearch> docGroup in documentGroups)
                {
                    ViewCrossPlanSearch doc = docGroup.First();
                    if (doc.FieldType == "date")
                    {
                        if (DateTime.TryParse(doc.FieldValue, out DateTime parsedDate))
                        {
                            doc.FieldValue = parsedDate.ToString("yyyy/MM/dd HH:mm");
                        }
                        else
                        {
                            doc.FieldValue = string.Empty;
                        }
                    }
                    detailData.Documents.Add(new ViewDocumentData
                    {
                        FieldName = doc.FieldName ?? string.Empty,
                        FieldValue = doc.FieldValue ?? string.Empty,
                        FieldType = doc.FieldType ?? string.Empty,
                        Unit = doc.Unit ?? string.Empty,
                        CustomName = doc.CustomName ?? string.Empty,
                        AreaName = doc.DataAreaName ?? string.Empty
                    });
                }
                planData.Details.Add(detailData);
            }
            result.Add(planData);
        }
        return result;
    }

    private static (SearchType, DisplayLevel) GetHighestCountType(int planCount, int detailCount, int fieldCount)
    {
        if (fieldCount >= planCount && fieldCount >= detailCount)
        {
            return (SearchType.FieldLevel, DisplayLevel.FullHierarchy);
        }
        else if (detailCount >= planCount)
        {
            return (SearchType.DetailLevel, DisplayLevel.PlanWithDetail);
        }
        else
        {
            return (SearchType.PlanLevel, DisplayLevel.PlanOnly);
        }
    }

    public async Task<IEnumerable<string>> GetPlanSearchPredict(string keyword, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return [];

        SqlBuilder sqlBuilder = new SqlBuilder(@"
            SELECT DISTINCT TOP 5 p.PlanName
            FROM [Plan] p
        ").WhereScope(where =>
        {
            where.AddWhereClause("AND p.PlanName COLLATE Chinese_Taiwan_Stroke_CI_AS LIKE @keyword");
            where.AddWhereClause("AND p.TenantID = @tenantId");
            where.AddWhereClause("AND p.Show = 1");
            where.AddWhereClause("AND p.Archived = 0");
        }).AddOrderBy("p.PlanName ASC");

        return await context.QueryAsync<string>(sqlBuilder.ToString(), new
        {
            keyword = $"%{keyword.Trim()}%",
            tenantId = tenantId
        });
    }
    public async Task<IEnumerable<string>> GetPlanSearchHistoriesAsync(string userId, string tenantId)
    {
        SqlBuilder sqlBuilder = new SqlBuilder(@"
            SELECT TOP 5 psh.KeyWord
            FROM [PlanSearchHistory] psh
        ").WhereScope(where =>
        {
            where.AddWhereClause("AND psh.UserID = @userId");
            where.AddWhereClause("AND psh.TenantID = @tenantId");
        }).AddGroupBy("psh.KeyWord")
        .AddOrderBy("MAX(psh.CreatedDate) DESC");

        return await context.QueryAsync<string>(sqlBuilder.ToString(), new { userId, tenantId });
    }

}