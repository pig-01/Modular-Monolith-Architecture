using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.SeedWork;

namespace Main.WebApi.Application.Queries.CustomTemplate;

public interface ICustomRequestUnitQuery : IQuery<CustomRequestUnit>
{
    Task<IEnumerable<CustomRequestUnit>> ListAsync(string tenantId, CancellationToken cancellationToken = default);
}
