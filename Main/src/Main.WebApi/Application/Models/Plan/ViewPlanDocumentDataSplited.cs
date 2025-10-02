using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlanDocumentDataSplited
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 計畫文件ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planDocumentId")]
    public int PlanDocumentId { get; set; }

    /// <summary>
    /// 文件ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("documentId")]
    public int DocumentId { get; set; }

    /// <summary>
    /// 年度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    /// <summary>
    /// 季度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("quarter")]
    public int? Quarter { get; set; }


    /// <summary>
    /// 月份
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("month")]
    public int? Month { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// 工廠名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("areaName")]
    public string? AreaName { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 租戶ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    /// <summary>
    /// 是否封存
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    /// <summary>
    /// 欄位ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("fieldId")]
    public required string FieldId { get; set; }

    /// <summary>
    /// 欄位名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fieldName")]
    public string? FieldName { get; set; }

    /// <summary>
    /// 欄位類型
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("fieldType")]
    public string? FieldType { get; set; }

    /// <summary>
    /// 分割值
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("splitValue")]
    public string? SplitValue { get; set; }

    /// <summary>
    /// 是否必填
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    /// <summary>
    /// 是否唯讀
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("readOnly")]
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// 週期編號
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleNumber")]
    public int? CycleNumber { get; set; }

    /// <summary>
    /// 週期類型
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    /// <summary>
    /// 自訂名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customName")]
    public string? CustomName { get; set; }

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
}
