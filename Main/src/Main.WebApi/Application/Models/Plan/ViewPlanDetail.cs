using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlanDetail
{
    /// <summary>
    /// 識別碼
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planDetailId")]
    public int PlanDetailId { get; set; }

    /// <summary>
    /// 計劃ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planId")]
    public int? PlanId { get; set; }

    /// <summary>
    /// 計畫範本ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planTemplateId")]
    public int? PlanTemplateId { get; set; }

    /// <summary>
    /// 自訂計畫範本ID
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("customPlanTemplateId")]
    public int? CustomPlanTemplateId { get; set; }

    /// <summary>
    /// 項目名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailName")]
    public string? PlanDetailName { get; set; }

    /// <summary>
    /// 項目名稱(中文)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailChName")]
    public string? PlanDetailChName { get; set; }

    /// <summary>
    /// 項目名稱(英文)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailEnName")]
    public string? PlanDetailEnName { get; set; }

    /// <summary>
    /// 項目名稱(日文)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailJpName")]
    public string? PlanDetailJpName { get; set; }

    /// <summary>
    /// 國際化項目名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("i18nPlanDetailName")]
    public string? I18nPlanDetailName { get; set; }

    /// <summary>
    /// bizform樣板ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("formId")]
    public int? FormId { get; set; }

    /// <summary>
    /// 資料來源
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("dataSource")]
    public string? DataSource { get; set; }


    /// <summary>
    /// 接受資料來源
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("acceptDataSource")]
    public string? AcceptDataSource { get; set; }

    /// <summary>
    /// api串接id
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("apiConnectionId")]
    public int? ApiConnectionId { get; set; }

    /// <summary>
    /// api串接結果
    /// </summary>
    [JsonPropertyName("apiConnectionFailed")]
    public bool ApiConnectionFailed { get; set; } = false;

    /// <summary>
    /// api串接報告id
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("netzeroReportId")]
    public int? NetzeroReportId { get; set; }

    /// <summary>
    /// api串接報告名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("netzeroReportName")]
    public string? NetzeroReportName { get; set; }

    /// <summary>
    /// 負責人清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsibleList")]
    public string? ResponsibleList { get; set; }

    /// <summary>
    /// 負責人名稱清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsibleNameList")]
    public string? ResponsibleNameList { get; set; }

    /// <summary>
    /// 分群ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("groupId")]
    public string? GroupId { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("rowNumber")]
    public string? RowNumber { get; set; }

    /// <summary>
    /// 週期類型year、month、quarter
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    /// <summary>
    /// 週期月份(每季第幾個月)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleMonth")]
    public int? CycleMonth { get; set; }

    /// <summary>
    /// 是否隱藏hint
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("showHint")]
    public bool? ShowHint { get; set; }

    /// <summary>
    /// 週期日(每季第幾天)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleDay")]
    public int? CycleDay { get; set; }

    /// <summary>
    /// 週期月份是否為最後一個月
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("cycleMonthLast")]
    public bool CycleMonthLast { get; set; }

    /// <summary>
    /// 是否顯示
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("show")]
    public bool Show { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 建檔時間
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("createdDate")]
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("createdUser")]
    public string? CreatedUser { get; set; }

    /// <summary>
    /// 最後修改時間
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedDate")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 最後修改人員
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedUser")]
    public string? ModifiedUser { get; set; }

    /// <summary>
    /// 分群名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("i18nGroupName")]
    public string? I18nGroupName { get; set; }

    /// <summary>
    /// 週期名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("i18nCycleTypeName")]
    public string? I18nCycleTypeName { get; set; }

    /// <summary>
    /// 是否有查看權限
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("hasViewPermission")]
    public bool HasViewPermission { get; set; }

    /// <summary>
    /// 是否有編輯權限
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("hasEditPermission")]
    public bool HasEditPermission { get; set; }

    /// <summary>
    /// Plan主檔計畫年度
    /// </summary>
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    /// <summary>
    /// 指標明細表單清單
    /// </summary>
    [JsonPropertyName("documents")]
    public List<ViewPlanDocument> PlanDocumentList { get; set; } = [];



}
