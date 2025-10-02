using System;
using System.Text.Json.Serialization;

namespace Main.Dto.ViewModel.Plan;

public class ViewPlanAreaData
{

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planId")]
    public int PlanId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planFactoryId")]
    public int PlanFactoryId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("comapnyId")]
    public long? CompanyId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("areaCode")]
    public string? AreaCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("areaName")]
    public string? AreaName { get; set; }
}
