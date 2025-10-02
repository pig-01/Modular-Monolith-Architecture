using Main.Domain.AggregatesModel.PlanAggregate;
using MediatR;

namespace Main.Domain.Events.PlanAggregate;

public class PlanArchivedDomainEvent(Plan plan, DateTime archiveDate, string archiveUser) : INotification
{
    public Plan Plan { get; } = plan;

    public DateTime ArchiveDate { get; } = archiveDate;

    public string ArchiveUser { get; } = archiveUser;
}
