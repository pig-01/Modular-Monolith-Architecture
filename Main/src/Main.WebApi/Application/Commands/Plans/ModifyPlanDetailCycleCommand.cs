using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class ModifyPlanDetailCycleCommand : IRequest<bool>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    [DefaultValue(1)]
    public int PlanDetailId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("cycleType")]
    [DefaultValue("year")]
    public string? CycleType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("cycleMonth")]
    [DefaultValue(null)]
    public int? CycleMonth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("cycleDay")]
    [DefaultValue(null)]
    public int? CycleDay { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("cycleMonthLast")]
    [DefaultValue(false)]
    public bool CycleMonthLast { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("endDate")]
    [DefaultValue("2026-11-30T23:59:59")]
    public DateTime EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("isApplyAll")]
    [DefaultValue(false)]
    public bool IsApplyAll { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDocumentCycleArray")]
    public List<PlanDocumentCycle> PlanDocumentCycleArray { get; set; } = [];
}

public class PlanDocumentCycle
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

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
}
