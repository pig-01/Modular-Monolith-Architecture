using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class QueryPlanDetailRequest
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planId")]
    public int? PlanID { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailName")]
    public string? PlanDetailName { get; set; }
}
