namespace DataHub.Infrastructure.Options;

public class DataHubOptions
{
    public const string Position = "DataHub";

    private string _skuPrefix = "demo_Demo";
    public string SkuPrefix { get => _skuPrefix; set => _skuPrefix = value ?? throw new NullReferenceException("Configure SkuPrefix is null"); }
    private string _trialSkuPrefix = "demo_Demo_trial";
    public string TrialSkuPrefix { get => _trialSkuPrefix; set => _trialSkuPrefix = value ?? throw new NullReferenceException("Configure TrialSkuPrefix is null"); }
    public bool PreserveLoginUrl { get; set; }
    public bool ClientValidationEnabled { get; set; }
    public string? DefaultTimeZoneId { get; set; }
    public string? SC30DB { get; set; }
    public string? APUrl { get; set; }
    public string? CanModifyCardTime { get; set; }
}
