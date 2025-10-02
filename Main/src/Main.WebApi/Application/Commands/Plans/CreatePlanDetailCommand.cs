using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class CreatePlanDetailCommand : IRequest<bool>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planId")]
    public int? PlanId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateId")]
    public int? PlanTemplateId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customPlanTemplateId")]
    public int? CustomPlanTemplateId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rowNumber")]
    public int RowNumber { get; set; }
}
