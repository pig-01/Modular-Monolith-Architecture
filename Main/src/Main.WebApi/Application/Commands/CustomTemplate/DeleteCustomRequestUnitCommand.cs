using System.Text.Json.Serialization;

namespace Main.WebApi.Application.Commands.CustomTemplate;

/// <summary>
/// 刪除要求單位命令
/// </summary>
public class DeleteCustomRequestUnitCommand(long requestUnitId) : IRequest<bool>
{
    [JsonPropertyName("requestUnitId")]
    public long RequestUnitId { get; set; } = requestUnitId;
}
