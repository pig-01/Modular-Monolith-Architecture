using System.Text.Json.Serialization;
using Main.Dto.Model.Pagination;

namespace Main.Dto.ViewModel.PlanSearch;

public class QueryPlanSearchRequest : SortedPaginationModel<QueryPlanSearchRequest>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("keyWord")]
    public string? KeyWord { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("startYear")]
    public int? StartYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("endYear")]
    public int? EndYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("areaCode")]
    public string? AreaCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tenantId")]
    public string? TenantID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("responsible")]
    public string Responsible { get; set; } = string.Empty;
}
