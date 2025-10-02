using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class SyncPlanTemplateFormIdCommand : IRequest<bool>
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
