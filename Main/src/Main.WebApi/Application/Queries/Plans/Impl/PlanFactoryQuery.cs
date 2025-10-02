using System;
using System.Linq.Expressions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Dto.ViewModel.Plan;
using Main.Infrastructure.Demo.Context;
using Microsoft.EntityFrameworkCore;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanFactoryQuery(DemoContext context) : BaseQuery, IPlanFactoryQuery
{

    private readonly DemoContext context = context;

    public Task<PlanFactory?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public async Task<IEnumerable<ViewPlanAreaData>> GetPlanAreaDataByPlanDetailId(int planDetailId, string tenantId, CancellationToken cancellationToken = default) => await (
        from pd in context.PlanDetails.AsNoTracking()
        join pf in context.PlanFactories on pd.PlanId equals pf.PlanId
        join plan in context.Plans on pf.PlanId equals plan.PlanId
        join area in context.Areas.Include(a => a.Company) on pf.FactoryId equals area.AreaCode
        where pd.PlanDetailId == planDetailId && pf.TenantId == tenantId && plan.CompanyId == area.CompanyId
        select new ViewPlanAreaData
        {
            PlanId = pd.PlanId,
            PlanFactoryId = pf.Id,
            AreaCode = area.AreaCode,
            AreaName = area.AreaName,
            CompanyId = area.CompanyId,
            CompanyName = area.Company.CompanyName
        })
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ViewPlanAreaData>> GetPlanAreaDataByPlanId(int planId, string tenantId, CancellationToken cancellationToken = default) => await (
        from pf in context.PlanFactories.AsNoTracking()
        join plan in context.Plans on pf.PlanId equals plan.PlanId
        join area in context.Areas.Include(a => a.Company) on pf.FactoryId equals area.AreaCode
        where pf.PlanId == planId && pf.TenantId == tenantId && plan.CompanyId == area.CompanyId
        select new ViewPlanAreaData
        {
            PlanId = pf.PlanId,
            PlanFactoryId = pf.Id,
            AreaCode = area.AreaCode,
            AreaName = area.AreaName,
            CompanyId = area.CompanyId,
            CompanyName = area.Company.CompanyName
        })
        .ToListAsync(cancellationToken);


    public Task<IEnumerable<PlanFactory>> ListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<IEnumerable<PlanFactory>> ListAsync(Expression<Func<PlanFactory, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
