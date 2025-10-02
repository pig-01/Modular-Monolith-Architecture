using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.CustomTemplate;

/// <summary>
/// 客戶自訂範本 Excel 匯入資料
/// </summary>
public class ViewCustomTemplateExcelData
{
    /// <summary>
    /// 指標分類
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 議題編號
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// 指標議題
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateName")]
    public string? PlanTemplateName { get; set; }

    /// <summary>
    /// 指標議題簡體中文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planTemplateNameCh")]
    public string? PlanTemplateNameCh { get; set; }

    /// <summary>
    /// 指標議題英文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planTemplateNameEn")]
    public string? PlanTemplateNameEn { get; set; }

    /// <summary>
    /// 指標議題日文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planTemplateNameJp")]
    public string? PlanTemplateNameJp { get; set; }

    /// <summary>
    /// 預設週期
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    /// <summary>
    /// demo bizform 樣板名稱
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formName")]
    public string? FormName { get; set; }

    /// <summary>
    /// 指標項目
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailTitle")]
    public string? PlanTemplateDetailTitle { get; set; }

    /// <summary>
    /// 指標項目簡體中文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailTitleCh")]
    public string? PlanTemplateDetailTitleCh { get; set; }

    /// <summary>
    /// 指標項目英文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailTitleEn")]
    public string? PlanTemplateDetailTitleEn { get; set; }

    /// <summary>
    /// 指標項目日文
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailTitleJp")]
    public string? PlanTemplateDetailTitleJp { get; set; }

    /// <summary>
    /// 產業必要揭露
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exposeIndustries")]
    public string? ExposeIndustries { get; set; }

    /// <summary>
    /// demo bizform 樣板 Id
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [JsonPropertyName("formId")]
    public long FormId { get; set; } = 0;
}
