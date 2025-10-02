using Scheduler.Domain.AggregateModel.PlanAggregate;
using Scheduler.Domain.SeedWork;

namespace Scheduler.Application.Queries.Plans;

public interface IPlanDocumentQuery : IQuery<PlanDocument>
{
    Task<IEnumerable<PlanDocument>> GetExpiringDocumentsAsync(int daysUntilExpiration, CancellationToken cancellationToken);
}
