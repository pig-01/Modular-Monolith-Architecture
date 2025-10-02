using Base.Infrastructure.Interface.Mail;
using Main.Domain.Events.PlanAggregate;

namespace Main.WebApi.Application.DomainEventHandlers.Plans;

public class PlanArchivedDomainEventHandler(ILogger<PlanArchivedDomainEventHandler> logger, IMailService mailService) : INotificationHandler<PlanArchivedDomainEvent>
{
    public async Task Handle(PlanArchivedDomainEvent notification, CancellationToken cancellationToken)
    {
        // 紀錄計劃已封存的事件在 logger 中
        logger.LogInformation("Plan with ID {PlanId} has been archived.", notification.Plan.PlanId);
        logger.LogInformation("Archived by user: {User}", notification.ArchiveUser);
        logger.LogInformation("Archive date: {Date}", notification.ArchiveDate);
        //logger.LogDebug("Plan details: {@Plan}", notification.Plan);



    }
}
