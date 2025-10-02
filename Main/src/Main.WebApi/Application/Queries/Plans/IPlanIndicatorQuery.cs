using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanIndicatorQuery : IQuery<PlanIndicator>
{
    Task<IEnumerable<PlanIndicator>> ListAsync(long requestUnitId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlanIndicator>> ListAsync(long requestUnitId, long versionId, CancellationToken cancellationToken = default);
}
