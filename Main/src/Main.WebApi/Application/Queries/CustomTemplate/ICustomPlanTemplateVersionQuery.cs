using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.SeedWork;

namespace Main.WebApi.Application.Queries.CustomTemplate;

public interface ICustomPlanTemplateVersionQuery : IQuery<CustomPlanTemplateVersion>
{
    Task<CustomPlanTemplateVersion?> GetByIdAsync(long versionId, string tenantId, CancellationToken cancellationToken = default);
}
