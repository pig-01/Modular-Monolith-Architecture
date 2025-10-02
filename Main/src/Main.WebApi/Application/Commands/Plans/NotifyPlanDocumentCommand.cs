using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class NotifyPlanDocumentCommand : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailIdList")]
    public required int[] PlanDetailIdList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    [DefaultValue(-1)]
    public required int PlanId { get; set; }
}
