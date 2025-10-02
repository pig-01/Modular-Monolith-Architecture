using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class AttachDocument2PlanDecumentCommand : IRequest<bool>
{

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    [DefaultValue(-1)]
    public int PlanDetailId { get; set; }

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("endDate")]
    public DateTime EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("quarter")]
    public int? Quarter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("month")]
    public int? Month { get; set; }

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("documentId")]
    [DefaultValue(1)]
    public int DocumentId { get; set; }

    [Required]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planFactoryId")]
    public int planFactoryId { get; set; }
}
