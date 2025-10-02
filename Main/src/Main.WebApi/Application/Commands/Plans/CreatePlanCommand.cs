using System.ComponentModel;
using System.Text.Json.Serialization;
using Base.Infrastructure.Toolkits.Extensions;

namespace Main.WebApi.Application.Commands.Plans;

public class CreatePlanCommand : IRequest<int>
{
    /// <summary>
    /// 計劃名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planName")]
    [DefaultValue("2025 凡奈斯金融保險永續指標")]
    public required string PlanName { get; set; }

    /// <summary>
    /// 計劃年度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    [DefaultValue(2025)]
    public int? Year { get; set; }

    /// <summary>
    /// 公司識別碼
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("companyId")]
    [DefaultValue("1")]
    public long CompanyId { get; set; }

    /// <summary>
    /// 指標清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("indicatorIdList")]
    [DefaultValue("1,2")]
    public string? IndicatorId { get; set; }

    /// <summary>
    /// 自訂指標
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customIndicatorIdList")]
    [DefaultValue("1,2")]
    public string? CustomIndicatorId { get; set; }

    /// <summary>
    /// 區域清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("factoryIdList")]
    [DefaultValue("F_01,F_02,F_03")]
    public string? FactoryId { get; set; }

    /// <summary>
    /// 產業清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("industryIdList")]
    [DefaultValue("GRI")]
    public string? IndustryId { get; set; }

    /// <summary>
    /// 計劃範本清單
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateIdList")]
    [DefaultValue(new int[] { 1, 2 })]
    public int[] PlanTemplateIdList { get; set; } = [];

    /// <summary>
    /// 自訂計劃範本清單
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customPlanTemplateIdList")]
    [DefaultValue(new int[] { 1, 2 })]
    public int[] CustomPlanTemplateIdList { get; set; } = [];

    /// <summary>
    /// 計劃範本版本
    /// </summary>
    /// <value></value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("planTemplateVersion")]
    public string? PlanTemplateVersion { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string[] IndicatorIdList => IndicatorId.IsNullOrEmpty() ? [] : IndicatorId!.Split(',');

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int[] CustomIndicatorIdList => CustomIndicatorId.IsNullOrEmpty() ? [] : [.. CustomIndicatorId!.Split(',').Select(int.Parse)];

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string[] FactoryIdList => FactoryId.IsNullOrEmpty() ? [] : FactoryId!.Split(',');

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string[] IndustryIdList => IndustryId.IsNullOrEmpty() ? [] : IndustryId!.Split(',');
}
