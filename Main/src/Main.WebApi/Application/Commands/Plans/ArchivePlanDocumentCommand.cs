using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class ArchivePlanDocumentCommand(int planDocumentId) : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDocumentId")]
    public int PlanDocumentId { get; set; } = planDocumentId;
}
