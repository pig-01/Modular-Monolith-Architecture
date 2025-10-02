using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlanDocument
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDocumentId")]
    public int? PlanDocumentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planDetailId")]
    public int? PlanDetailId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("documentId")]
    public int? DocumentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planFactoryId")]
    public int? PlanFactoryId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsible")]
    public string? Responsible { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("approve")]
    public string? Approve { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formStatus")]
    public string? FormStatus { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("createdDate")]
    public DateTime? CreatedDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("createdUser")]
    public string? CreatedUser { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedDate")]
    public DateTime? ModifiedDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedUser")]
    public string? ModifiedUser { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("quarter")]
    public int? Quarter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("month")]
    public int? Month { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cycleType")]
    public string? CycleType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsibleName")]
    public string? ResponsibleName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("approveName")]
    public string? ApproveName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formStatusName")]
    public string? FormStatusName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("i18nFormStatusName")]
    public string? I18nFormStatusName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("formStatusColor")]
    public string? FormStatusColor { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("hasViewPermission")]
    public bool HasViewPermission { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("hasEditPermission")]
    public bool HasEditPermission { get; set; }
}
