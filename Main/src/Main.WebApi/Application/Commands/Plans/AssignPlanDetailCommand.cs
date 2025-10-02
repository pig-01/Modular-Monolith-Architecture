using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class AssignPlanDetailCommand : IRequest<Unit>
{

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("responsibleList")]
    [DefaultValue("jason_tsai@Demo.com.tw")]
    public required string ResponsibleList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailIdList")]
    public required int[] PlanDetailIdList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    public required int PlanId { get; set; }
}