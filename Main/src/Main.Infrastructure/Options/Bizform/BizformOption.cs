using System.Text.Json.Serialization;

namespace Main.Infrastructure.Options.Bizform;

public class BizformOption
{
    public const string Position = "Bizform";

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Url")]
    public required string Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Authorization")]
    public required AuthorizationSetting Authorization { get; set; }
}
