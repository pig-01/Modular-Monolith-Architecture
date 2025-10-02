using System;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanFactoryQuery : IQuery<PlanFactory>
{
    Task<IEnumerable<ViewPlanAreaData>> GetPlanAreaDataByPlanId(int planId, string tenantId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ViewPlanAreaData>> GetPlanAreaDataByPlanDetailId(int planDetailId, string tenantId, CancellationToken cancellationToken = default);

}
