using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanDocumentDataSplitedQuery : IQuery<PlanDocumentDataSplited>
{
    Task<IEnumerable<int>> GetYearRangeAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ViewPlanDocumentDataSplited>> GetDtoByDashboardArgsAsync(QueryPlanDocumentDataSplitedRequest req, CancellationToken cancellationToken = default);
    Task<IEnumerable<ViewPlanDocumentDataSplited>> GetDtoByPlanDocumentIdAsync(int planDocumentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ViewPlanDocumentDataSplitConfig>> GetConfigListDtoAsync(CancellationToken cancellationToken = default);

}
