using System.Text.Json.Serialization;
using Main.Infrastructure.Options.Bizform;

namespace Main.Infrastructure.Options.NetZero;

public class NetZeroOption
{
    public const string Position = "NetZero";

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Url")]
    public required string Url { get; set; }
}
