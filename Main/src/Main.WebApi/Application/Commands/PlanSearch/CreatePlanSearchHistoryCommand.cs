using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.PlanSearch;

public class CreatePlanSearchHistoryCommand : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("keyWord")]
    public required string KeyWord { get; set; }
}
