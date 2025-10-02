using System.Linq.Expressions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;
using Main.Infrastructure.Demo.Context;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanDocumentLegacyQuery(DemoContext context) : IPlanDocumentLegacyQuery
{

    public async Task<IEnumerable<ViewPlanDocumentLegacy>> GetDtoByDetailIdAsync(int detailId, CancellationToken cancellationToken = default) => await context.QueryAsync<ViewPlanDocumentLegacy>(@"
            SELECT PlanDocumentID
                ,PlanDetailID
                ,DocumentID
                ,Responsible
                ,ResponsibleName
                ,Approve
                ,ApproveName
                ,FormStatus
                ,I18nFormStatusName
                ,FormStatusColor
                ,StartDate
                ,EndDate
                ,Year
                ,Quarter
                ,Month
                ,CycleType
                ,PlanFactoryID
                ,AreaCode
                ,AreaName
                ,CreatedDate
                ,CreatedUser
                ,ModifiedDate
                ,ModifiedUser
                ,ArchivedDate
                ,ArchivedUser
            FROM vwPlanDocumentLegacy
            WHERE PlanDetailID = @PlanDetailID
        ", new { PlanDetailID = detailId });

    public Task<IEnumerable<PlanDocumentLegacy>> ListAsync(Expression<Func<PlanDocumentLegacy, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IEnumerable<ViewPlanDocument>> ListDtoAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    Task<PlanDocumentLegacy?> IQuery<PlanDocumentLegacy>.GetByIdAsync(long id, CancellationToken cancellationToken) => throw new NotImplementedException();

    Task<IEnumerable<PlanDocumentLegacy>> IQuery<PlanDocumentLegacy>.ListAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
