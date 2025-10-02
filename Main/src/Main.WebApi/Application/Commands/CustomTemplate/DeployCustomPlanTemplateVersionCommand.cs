using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 發布自訂計畫範本版本命令
/// </summary>
public class DeployCustomPlanTemplateVersionCommand(long requestUnitId, long versionId) : IRequest<bool>
{
    [JsonPropertyName("requestUnitId")]
    public long RequestUnitId { get; } = requestUnitId;

    [JsonPropertyName("versionId")]
    public long VersionId { get; } = versionId;
}
