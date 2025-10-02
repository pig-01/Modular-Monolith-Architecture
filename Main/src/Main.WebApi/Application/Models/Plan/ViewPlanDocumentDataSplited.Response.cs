using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlanDocumentDataSplitedYearRange
{
    /// <summary>
    /// 主鍵ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("minYear")]
    public int MinYear { get; set; }

    /// <summary>
    /// 計畫文件ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("maxYear")]
    public int MaxYear { get; set; }

}
