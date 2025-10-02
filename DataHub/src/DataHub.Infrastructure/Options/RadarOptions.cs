namespace DataHub.Infrastructure.Options;

public class RadarOptions
{
    public const string Position = "Radar";

    public string? RadarKiss { get; set; }

    public List<string>? SuperUserIp { get; set; }

    public List<string>? ValidateIp { get; set; }

    public Utility? Utility { get; set; }
}

public class Utility
{
    public bool Usevault { get; set; }

    public string? Rooturl { get; set; }

    public string? Token { get; set; }
}