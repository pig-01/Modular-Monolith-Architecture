using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.Plans;

public class AssignPlanDocumentCommand : IRequest<Unit>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("dataList")]
    public required List<AssignPlanDetail> DataList { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("responsiblePerson")]
    [DefaultValue("jason_tsai@Demo.com.tw")]
    public required string ResponsiblePerson { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    [DefaultValue(-1)]
    public required int PlanId { get; set; }
}

public struct AssignPlanDetail()
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planDetailId")]
    [DefaultValue(-1)]
    public required int PlanDetailId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("month")]
    [DefaultValue(4)]
    public int? Month { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("quarter")]
    [DefaultValue(null)]
    public int? Quarter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    [DefaultValue(null)]
    public int? Year { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    internal readonly bool IsSingleMonth => Month != null && Quarter == null && Year == null;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    internal readonly bool IsSingleQuarter => Month == null && Quarter != null && Year == null;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    internal readonly bool IsSingleYear => Month == null && Quarter == null && Year != null;
}
