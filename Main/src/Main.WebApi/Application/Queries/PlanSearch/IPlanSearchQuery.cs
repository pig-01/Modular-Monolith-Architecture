using Base.Domain.SeedWorks.MediatR;
using Main.Domain.AggregatesModel.PlanSearchAggreate;
using Main.Dto.ViewModel.PlanSearch;
using Main.WebApi.Application.Queries.PlanSearch.Impl;

namespace Main.WebApi.Application.Queries.PlanSearch;

public interface IPlanSearchQuery : IQuery<PlanSearchQuery>
{
    Task<List<ViewPlanSearchTreeData>> SearchPlansAsync(QueryPlanSearchRequest request);

    Task<IEnumerable<string>> GetPlanSearchHistoriesAsync(string userId, string tenantId);

    Task<IEnumerable<string>> GetPlanSearchPredict(string keyword, string tenantId);
}
