using System.Text.Json.Serialization;

namespace Main.Infrastructure.Options.Api;

public class CorsOption
{
    public const string Position = "CORS";

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("CorsPolicyName")]
    public required string CorsPolicyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("PolicySettings")]
    public required List<PolicySetting> PolicySettings { get; set; }
}
