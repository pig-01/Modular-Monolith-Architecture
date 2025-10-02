using System.Text.Json.Serialization;
using Main.WebApi.Application.Models.Bizform.Accounts;
using Main.WebApi.Application.Models.NetZero;

namespace Main.WebApi.Application.Commands.Plans;

public class ModifyPlanDocumentDataByNetZeroCommand : IRequest<NetZeroResponse>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    public int PlanDetailId { get; set; }

}


