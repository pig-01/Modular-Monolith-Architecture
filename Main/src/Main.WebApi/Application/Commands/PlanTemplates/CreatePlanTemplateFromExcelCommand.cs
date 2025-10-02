using System.Text.Json.Serialization;
using Main.Dto.ViewModel.PlanTemplate;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class CreatePlanTemplateFromExcelCommand : IRequest<bool>
{

    public required string Version { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("dataList")]
    public required List<ViewPlanTemplateExcelData> DataList { get; set; }
}