using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class CancelConnectNetZeroToPlanDetailCommand() : IRequest<Unit>
{

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    public int PlanDetailId { get; set; }
}
