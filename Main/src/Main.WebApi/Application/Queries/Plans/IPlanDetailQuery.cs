using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanDetailQuery : IQuery<PlanDetail>
{

    Task<IEnumerable<PlanDetail>> GetByIdListAsync(int[] idList, CancellationToken cancellationToken = default);

    Task<ViewPlanDetail?> GetDtoByIdAsync(int id, string responsible, CancellationToken cancellationToken = default);

    Task<IEnumerable<ViewPlanDetail>> GetDtoByPlanIdAsync(int planId, string responsible, CancellationToken cancellationToken = default);

    Task<IEnumerable<PlanDetail>> ListByPlanIdAsync(int planId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ViewPlanDetail>> ListDtoAsync(QueryPlanDetailRequest request, CancellationToken cancellationToken = default);
}
