using System.Linq.Expressions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanIndicatorQuery(DemoContext context) : BaseQuery, IPlanIndicatorQuery
{
    private readonly DemoContext context = context;

    public Task<PlanIndicator?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => context.PlanIndicators.AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<IEnumerable<PlanIndicator>> ListAsync(long requestUnitId, CancellationToken cancellationToken = default) => await context.PlanIndicators.AsNoTracking()
        .Where(x => x.RequestUnitId == requestUnitId)
        .ToListAsync(cancellationToken);
    public async Task<IEnumerable<PlanIndicator>> ListAsync(long requestUnitId, long versionId, CancellationToken cancellationToken = default) => await context.PlanIndicators.AsNoTracking()
        .Where(x => x.RequestUnitId == requestUnitId && x.VersionId == versionId)
        .ToListAsync(cancellationToken);
    public async Task<IEnumerable<PlanIndicator>> ListAsync(CancellationToken cancellationToken = default) => await context.PlanIndicators.AsNoTracking()
        .ToListAsync(cancellationToken);
    public async Task<IEnumerable<PlanIndicator>> ListAsync(Expression<Func<PlanIndicator, bool>> predicate, CancellationToken cancellationToken = default) => await context.PlanIndicators.AsNoTracking()
        .Include(x => x.Plan)
        .Where(predicate)
        .ToListAsync(cancellationToken);
}