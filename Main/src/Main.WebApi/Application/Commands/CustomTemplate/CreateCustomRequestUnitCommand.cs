using System.Text.Json.Serialization;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 新增要求單位命令
/// </summary>
/// <remarks>
/// 範例: 新增要求單位和版本 { "requestUnitName": "要求單位名稱", "version": "1.0" }
/// 範例: 只新增版本 { "requestUnitId": 1, "version": "1.1" }
/// </remarks>
public class CreateCustomRequestUnitCommand : IRequest<CustomRequestUnit>
{
    [JsonPropertyName("requestUnitId")]
    public long? RequestUnitId { get; set; }

    [JsonPropertyName("requestUnitName")]
    public string? RequestUnitName { get; set; }

    [JsonPropertyName("versionName")]
    public required string Version { get; set; }
}
