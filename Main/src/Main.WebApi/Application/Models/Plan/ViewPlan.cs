using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlan
{
    /// <summary>
    /// 計劃ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planId")]
    public int PlanId { get; set; }

    /// <summary>
    /// 計劃名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planName")]
    public required string PlanName { get; set; }

    /// <summary>
    /// 計劃年度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    public string? Year { get; set; }

    /// <summary>
    /// 公司
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("companyId")]
    public long? CompanyId { get; set; }

    /// <summary>
    /// 計劃年度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    /// <summary>
    /// 區域清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("factoryIdList")]
    public string? FactoryIdList { get; set; }

    /// <summary>
    /// 指標清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("indicatorIdList")]
    public string? IndicatorIdList { get; set; }

    /// <summary>
    /// 自訂指標清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customIndicatorIdList")]
    public string? CustomIndicatorIdList { get; set; }

    /// <summary>
    /// 產業清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("industryIdList")]
    public string? IndustryIdList { get; set; }

    /// <summary>
    /// 是否顯示
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("show")]
    public bool Show { get; set; }

    /// <summary>
    /// 計畫樣板版本
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateVersion")]
    public string? PlanTemplateVersion { get; set; }

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


    // view欄位

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
    /// 是否有更改計畫權限頁面權限
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("hasSettingPermission")]
    public bool HasSettingPermission { get; set; }

    /// <summary>
    /// 待辦數量
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("todoCount")]
    public int TodoCount { get; set; }

    /// <summary>
    /// 總數量
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// 待填寫數量
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("pendingWriteCount")]
    public int PendingWriteCount { get; set; }

    /// <summary>
    /// 待覆核數量
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("pendingReviewCount")]
    public int PendingReviewCount { get; set; }
}
