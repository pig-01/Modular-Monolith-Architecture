using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class DeployPlanTemplateCommand : IRequest<bool>
{
    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("version")]
    public required string Version { get; set; }
}