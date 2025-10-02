using System.Text.Json.Serialization;

namespace Main.Infrastructure.Options.Api;

public class PolicySetting
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("PolicyName")]
    public required string PolicyName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Origins")]
    public required string[] Origins { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Headers")]
    public required string[] Headers { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Methods")]
    public required string[] Methods { get; set; }
}
