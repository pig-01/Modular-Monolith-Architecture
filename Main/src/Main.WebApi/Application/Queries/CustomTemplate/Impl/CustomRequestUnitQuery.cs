using System.Linq.Expressions;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.CustomTemplate;

public class CustomRequestUnitQuery(DemoContext context) : ICustomRequestUnitQuery
{
    public async Task<CustomRequestUnit?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.CustomRequestUnits.AsNoTracking().FirstOrDefaultAsync(x => x.UnitId == id, cancellationToken);

    public async Task<IEnumerable<CustomRequestUnit>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.CustomRequestUnits.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<CustomRequestUnit>> ListAsync(Expression<Func<CustomRequestUnit, bool>> predicate, CancellationToken cancellationToken = default) =>
        await context.CustomRequestUnits.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<IEnumerable<CustomRequestUnit>> ListAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await context.CustomRequestUnits.AsNoTracking()
            .Include(x => x.CustomPlanTemplateVersions)
            .ThenInclude(x => x.CustomPlanTemplates)
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);

    }


}
