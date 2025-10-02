using System.Text.Json.Serialization;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 重新命名要求單位或版本命令
/// </summary>
public class RenameCustomRequestUnitCommand : IRequest<CustomRequestUnit>
{
    /// <summary>
    /// 要求單位識別碼
    /// </summary>
    /// <value></value>
    [JsonPropertyName("requestUnitId")]
    public long? RequestUnitId { get; set; }

    /// <summary>
    /// 要求單位名稱
    /// </summary>
    /// <value></value>
    [JsonPropertyName("requestUnitName")]
    public string? RequestUnitName { get; set; }

    /// <summary>
    /// 版本ID
    /// </summary>
    /// <value></value>
    [JsonPropertyName("versionId")]
    public long? VersionId { get; set; }

    /// <summary>
    /// 版本名稱
    /// </summary>
    /// <value></value>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}
