using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class ModifyPlanCommand : IRequest<bool>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    public int PlanId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planName")]
    public required string PlanName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    public int? Year { get; set; }
}
