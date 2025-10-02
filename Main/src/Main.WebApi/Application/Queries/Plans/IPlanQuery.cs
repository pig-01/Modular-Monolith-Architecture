using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.Model.Pagination;
using Main.Dto.ViewModel.Plan;
using Main.Dto.ViewModel.Widget;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanQuery : IQuery<Plan>
{
    Task<ViewPlan?> GetDtoByIdAsync(int id, string responsible, CancellationToken cancellationToken = default);
    Task<IEnumerable<Plan>> GetByIdsAsync(IEnumerable<int> planIds, CancellationToken cancellationToken = default);
    Task<PaginationResult<ViewPlan>> ListDtoAsync(QueryPlanRequest req, CancellationToken cancellationToken = default);
    Task<IEnumerable<ViewPlanExportDataSet>> GetExportDataSetAsync(string[] planDetailIds, CancellationToken cancellationToken = default);
    Task<PaginationResult<ViewMultiplePlanWidget>> GetMultiplePlanWidgetDataAsync(ViewPlanWidgetRequest req);
    Task<PaginationResult<ViewSinglePlanWidget>> GetPlanWidgetDataAsync(ViewPlanWidgetRequest req);
    Task<IEnumerable<string>> GetYearListAsync(string tenantId, CancellationToken cancellationToken = default);
}
