using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class CancelMultiplePlanCommand(params int[] planIds) : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    public int[] PlanIds { get; set; } = planIds;

    public DateTime ModifiedDate { get; set; }

    public string ModifiedUser { get; set; } = "System";

}
