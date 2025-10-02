using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.PlanSearch;

public record ViewPlanSearchTreeData
{
    [JsonPropertyName("planId")]
    public int PlanId { get; init; }

    [JsonPropertyName("planName")]
    public required string PlanName { get; init; }

    [JsonPropertyName("planAreaList")]
    public List<string> PlanAreaList { get; init; } = [];

    [JsonPropertyName("planYear")]
    public int PlanYear { get; init; }

    [JsonPropertyName("companyName")]
    public required string CompanyName { get; init; }

    [JsonPropertyName("details")]
    public List<ViewPlanDetailData> Details { get; init; } = [];
}

public record ViewPlanDetailData
{
    [JsonPropertyName("planDetailName")]
    public required string PlanDetailName { get; init; }

    [JsonPropertyName("planDetailId")]
    public int? PlanDetailId { get; init; }

    [JsonPropertyName("cycleType")]
    public required string CycleType { get; init; }

    [JsonPropertyName("cycleNumber")]
    public int CycleNumber { get; init; }

    [JsonPropertyName("rowIdNumber")]
    public required string RowIdNumber { get; init; }

    [JsonPropertyName("documents")]
    public List<ViewDocumentData> Documents { get; init; } = [];
}

public record ViewDocumentData
{
    [JsonPropertyName("fieldName")]
    public required string FieldName { get; init; }

    [JsonPropertyName("fieldValue")]
    public required string FieldValue { get; init; }

    [JsonPropertyName("fieldType")]
    public required string FieldType { get; init; }

    [JsonPropertyName("unit")]
    public required string Unit { get; init; }

    [JsonPropertyName("customName")]
    public required string CustomName { get; init; }

    [JsonPropertyName("areaName")]
    public required string AreaName { get; init; }
}