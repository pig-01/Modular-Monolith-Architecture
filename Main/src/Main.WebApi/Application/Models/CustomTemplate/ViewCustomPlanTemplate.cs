using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ViewCustomPlanTemplate
{
    /// <summary>
    /// 指標計畫樣版識別碼
    /// </summary>
    [JsonPropertyName("planTemplateId")]
    public int PlanTemplateId { get; set; }

    /// <summary>
    /// 指標計畫樣版版本識別碼
    /// </summary>
    [JsonPropertyName("versionId")]
    public long VersionId { get; set; }

    /// <summary>
    /// 指標計畫樣版名稱
    /// </summary>
    [JsonPropertyName("planTemplateName")]
    public string PlanTemplateName { get; set; } = null!;

    /// <summary>
    /// 指標計畫樣版名稱簡體中文
    /// </summary>
    [JsonPropertyName("planTemplateNameCh")]
    public string PlanTemplateNameCh { get; set; } = null!;

    /// <summary>
    /// 指標計畫樣版名稱英文
    /// </summary>
    [JsonPropertyName("planTemplateNameEn")]
    public string PlanTemplateNameEn { get; set; } = null!;

    /// <summary>
    /// 指標計畫樣版名稱日文
    /// </summary>
    [JsonPropertyName("planTemplateNameJp")]
    public string PlanTemplateNameJp { get; set; } = null!;

    /// <summary>
    /// 表單識別碼
    /// </summary>
    [JsonPropertyName("formId")]
    public int? FormId { get; set; }

    /// <summary>
    /// 表單名稱
    /// </summary>
    [JsonPropertyName("formName")]
    public string FormName { get; set; } = null!;

    /// <summary>
    /// 分群識別碼
    /// </summary>
    [JsonPropertyName("groupId")]
    public string GroupId { get; set; } = null!;

    /// <summary>
    /// 分群名稱
    /// </summary>
    /// <value></value>
    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 分群名稱
    /// </summary>
    /// <value></value>
    [JsonPropertyName("i18nGroupName")]
    public string? I18nGroupName { get; set; }

    /// <summary>
    /// 週期類型
    /// </summary>
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    /// <summary>
    /// 週期類型顯示名稱
    /// </summary>
    /// <value></value>
    [JsonPropertyName("cycleTypeDisplay")]
    public string? CycleTypeDisplay { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    [JsonPropertyName("sortSequence")]
    public int? SortSequence { get; set; }

    /// <summary>
    /// 指標計畫樣版明細
    /// </summary>
    [JsonPropertyName("customPlanTemplatesDetails")]
    public ICollection<ViewCustomPlanTemplateDetail> CustomPlanTemplateDetails { get; set; } = [];
}
