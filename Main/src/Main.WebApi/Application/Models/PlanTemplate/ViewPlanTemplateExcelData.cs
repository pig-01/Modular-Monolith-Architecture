using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.PlanTemplate;

public class ViewPlanTemplateExcelData
{
    //Demo平台議題(分類)
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    // 計畫範本名稱zh
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateName")]
    public string? PlanTemplateName { get; set; }

    // 計畫範本名稱ch
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateChName")]
    public string? PlanTemplateChName { get; set; }

    // 計畫範本名稱en
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateEnName")]
    public string? PlanTemplateEnName { get; set; }

    // 計畫範本名稱jp
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateJpName")]
    public string? PlanTemplateJpName { get; set; }

    // bizform樣板ID
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formId")]
    public int? FormId { get; set; }

    // bizform樣板名稱
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formName")]
    public string? FormName { get; set; }

    // 接受資料源
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("acceptDataSource")]
    public string? AcceptDataSource { get; set; }

    // 預設週期
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    // 是否已發佈
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("isDeploy")]
    public bool? IsDeploy { get; set; }

    // 計畫範本明細標題
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailTitle")]
    public string? PlanTemplateDetailTitle { get; set; }

    // 計畫範本明細標題ch
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailChTitle")]
    public string? PlanTemplateDetailChTitle { get; set; }

    // 計畫範本明細標題en
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailEnTitle")]
    public string? PlanTemplateDetailEnTitle { get; set; }

    // 計畫範本明細標題jp
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateDetailJpTitle")]
    public string? PlanTemplateDetailJpTitle { get; set; }

    // 要求單位清單
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("requestUnitIds")]
    public string? RequestUnitIds { get; set; }

    // 準則代碼清單
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("griRuleCodes")]
    public string? GriRuleCodes { get; set; }

    // 產業必要揭露清單
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("exposeIndustryIds")]
    public string? ExposeIndustryIds { get; set; }
}
