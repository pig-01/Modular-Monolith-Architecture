using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class HidePlanDetailHintCommand() : IRequest<Unit>
{

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    public int PlanDetailId { get; set; }
}
