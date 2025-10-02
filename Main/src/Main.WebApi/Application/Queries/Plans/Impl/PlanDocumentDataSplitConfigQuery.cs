using System.Linq.Expressions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanDocumentDataSplitConfigQuery(DemoContext context) : IPlanDocumentDataSplitConfigQuery
{
    public Task<PlanDocumentDataSplitConfig?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        context.PlanDocumentDataSplitConfigs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<PlanDocumentDataSplitConfig>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.PlanDocumentDataSplitConfigs.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlanDocumentDataSplitConfig>> ListAsync(
        Expression<Func<PlanDocumentDataSplitConfig, bool>> predicate, CancellationToken cancellationToken = default) =>
        await context.PlanDocumentDataSplitConfigs.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
}
