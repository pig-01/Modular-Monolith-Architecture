using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class HideMultiplePlanCommand(params int[] planIds) : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planId")]
    public int[] PlanIds { get; } = planIds;

    public DateTime ModifiedDate { get; set; }

    public string ModifiedUser { get; set; } = "System";

}
