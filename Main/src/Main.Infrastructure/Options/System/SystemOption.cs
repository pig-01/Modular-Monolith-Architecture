using System.Text.Json.Serialization;

namespace Main.Infrastructure.Options.System;

public class SystemOption
{
    public const string Position = "System";

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("Activation")]
    public required ActivationSettings Activation { get; set; }
}