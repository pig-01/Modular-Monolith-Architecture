using Main.Domain.AggregatesModel.PlanAggregate;
using MediatR;

namespace Main.Domain.Events.PlanAggregate;

public class PlanDocumentArchivedDomainEvent(PlanDocument planDocument, DateTime archiveDate, string archiveUser) : INotification
{
    public PlanDocument PlanDocument { get; } = planDocument;

    public DateTime ArchiveDate { get; } = archiveDate;

    public string ArchiveUser { get; } = archiveUser;
}
