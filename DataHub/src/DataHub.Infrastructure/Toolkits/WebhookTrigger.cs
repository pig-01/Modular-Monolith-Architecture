namespace DataHub.Infrastructure.Toolkits;

public class WebhookTrigger
{
    public const string DocumentCreated = "DocumentCreated";
    public const string DocumentUpdated = "DocumentUpdated";
    public const string DocumentDeleted = "DocumentDeleted";
    public const string WorkflowUpdated = "WorkflowUpdated";

    public static bool IsWebhookTrigger(string? name) => name switch
    {
        DocumentCreated => true,
        DocumentUpdated => true,
        DocumentDeleted => true,
        WorkflowUpdated => true,
        _ => false
    };
}
