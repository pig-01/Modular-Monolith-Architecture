using System.Text.Json.Serialization;
using Main.Dto.Model.Pagination;

namespace Main.Dto.ViewModel.Plan;

public class QueryPlanRequest : SortedPaginationModel<QueryPlanRequest>
{
    /// <summary>
    /// 計畫名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planName")]
    public string? PlanName { get; set; }

    /// <summary>
    /// TenantID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tenantId")]
    public string? TenantID { get; set; }

    /// <summary>
    /// 指派人
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsible")]
    public string Responsible { get; set; } = string.Empty;

    /// <summary>
    /// 是否完成
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("isCompleted")]
    public bool? IsCompleted { get; set; } = null;

    /// <summary>
    /// 是否顯示隱藏資料
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("showHiddenData")]
    public bool ShowHiddenData { get; set; } = false;
}
