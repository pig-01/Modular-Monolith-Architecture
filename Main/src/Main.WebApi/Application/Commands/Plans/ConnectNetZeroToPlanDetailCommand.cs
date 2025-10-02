using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class ConnectNetZeroToPlanDetailCommand() : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("apiConnectionId")]
    public int ApiConnectionId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("netZeroReportId")]
    public int NetZeroReportId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("netZeroReportName")]
    public string NetZeroReportName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    public int PlanDetailId { get; set; }
}
