using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ViewCustomPlanTemplateDetail
{
    /// <summary>
    /// 指標計畫樣版明細識別碼
    /// </summary>
    [JsonPropertyName("planTemplateDetailId")]
    public int PlanTemplateDetailId { get; set; }

    /// <summary>
    /// 指標計畫樣板識別碼
    /// </summary>
    [JsonPropertyName("planTemplateId")]
    public int PlanTemplateId { get; set; }

    /// <summary>
    /// 樣版明細名稱
    /// </summary>
    [JsonPropertyName("planTemplateDetailTitle")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 樣版明細名稱簡體中文
    /// </summary>
    [JsonPropertyName("planTemplateDetailTitleCh")]
    public string? TitleCh { get; set; }

    /// <summary>
    /// 樣版明細名稱英文
    /// </summary>
    [JsonPropertyName("planTemplateDetailTitleEn")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 樣版明細名稱日文
    /// </summary>
    [JsonPropertyName("planTemplateDetailTitleJp")]
    public string? TitleJp { get; set; }

    /// <summary>
    /// 排序編號
    /// </summary>
    [JsonPropertyName("sortSequence")]
    public int? SortSequence { get; set; }

    /// <summary>
    /// 自訂指標計畫樣版
    /// </summary>
    [JsonPropertyName("customPlanTemplate")]
    public ViewCustomPlanTemplate CustomPlanTemplate { get; set; } = null!;

}
