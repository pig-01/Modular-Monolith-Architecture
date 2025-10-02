using Main.Domain.Events.PlanAggregate;

namespace Main.WebApi.Application.DomainEventHandlers.Plans;

public class PlanDocumentArchivedDomainEventHandler(
    ILogger<PlanDocumentArchivedDomainEventHandler> logger) : INotificationHandler<PlanDocumentArchivedDomainEvent>
{
    public async Task Handle(PlanDocumentArchivedDomainEvent notification, CancellationToken cancellationToken)
    {
        // 紀錄計劃已封存的事件在 logger 中
        logger.LogInformation("PlanDocument with ID {PlanDocumentId} has been archived.", notification.PlanDocument.PlanDocumentId);
        logger.LogInformation("Archived by user: {User}", notification.ArchiveUser);
        logger.LogInformation("Archive date: {Date}", notification.ArchiveDate);
        logger.LogDebug("PlanDocument details: {@PlanDocument}", notification.PlanDocument);

        // Handle the event here
        await Task.CompletedTask;
    }
}