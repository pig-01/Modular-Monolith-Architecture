using System.Text.Json.Serialization;

namespace Main.Infrastructure.Options.Bizform;

public class AuthorizationSetting
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Scheme")]
    public required string Scheme { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Token")]
    public required string Token { get; set; }
}
