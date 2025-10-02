using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 刪除自訂計畫範本版本命令
/// </summary>
public class DeleteCustomPlanTemplateVersionCommand(long requestUnitId, long versionId) : IRequest<bool>
{

    [JsonPropertyName("requestUnitId")]
    public long RequestUnitId { get; set; } = requestUnitId;

    [JsonPropertyName("versionId")]
    public long VersionId { get; set; } = versionId;
}
