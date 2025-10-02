using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanDocumentLegacyQuery : IQuery<PlanDocumentLegacy>
{
    Task<IEnumerable<ViewPlanDocumentLegacy>> GetDtoByDetailIdAsync(int detailId, CancellationToken cancellationToken = default);
}
