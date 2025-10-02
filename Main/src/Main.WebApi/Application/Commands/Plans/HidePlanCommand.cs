using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class HidePlanCommand(int planId) : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("planId")]
    public int PlanId { get; } = planId;

    public DateTime ModifiedDate { get; set; }

    public string ModifiedUser { get; set; } = "System";

}
