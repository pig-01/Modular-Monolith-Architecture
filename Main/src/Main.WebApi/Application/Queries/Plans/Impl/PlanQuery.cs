using System.Linq.Expressions;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Utilities.SqlBuilder;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Dto.Model.Pagination;
using Main.Dto.ViewModel.Plan;
using Main.Dto.ViewModel.Widget;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanQuery(DemoContext context) : BaseQuery, IPlanQuery
{
    private readonly DemoContext context = context;

    public async Task<Plan?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => await context.Plans.Include(x => x.PlanDetails).FirstOrDefaultAsync(x => x.PlanId == id, cancellationToken);

    public async Task<IEnumerable<Plan>> GetByIdsAsync(IEnumerable<int> planIds, CancellationToken cancellationToken = default) => await context.Plans.Where(p => planIds.Contains(p.PlanId)).ToListAsync(cancellationToken);

    public async Task<ViewPlan?> GetDtoByIdAsync(int id, string responsible, CancellationToken cancellationToken = default) => await context.QueryFirstOrDefaultAsync<ViewPlan>(@"
            SELECT
	            pn.PlanID,
	            pn.PlanName,
	            pn.Year,
                pn.CompanyID,
                pn.TenantID,
                pn.FactoryIdList,
                pn.IndicatorIdList,
                pn.CustomIndicatorIdList,
                pn.IndustryIdList,
                pn.TodoCount,
                pn.TotalCount,
                pn.PendingWriteCount,
                pn.PendingReviewCount,
                pn.Show,
                pn.PlanTemplateVersion,
                pn.HasViewPermission,
                pn.HasEditPermission,
                pn.HasSettingPermission,
		        pn.CreatedDate,
		        pn.CreatedUser,
		        pn.ModifiedDate,
		        pn.ModifiedUser
            FROM [dbo].[fnGetPlanWithPermission](@Responsible) pn
            WHERE pn.PlanID = @PlanID And pn.HasViewPermission = 1", new { PlanID = id, Responsible = responsible });

    public async Task<IEnumerable<Plan>> ListAsync(CancellationToken cancellationToken = default) => await context.Plans.ToListAsync(cancellationToken);

    public async Task<IEnumerable<Plan>> ListAsync(Expression<Func<Plan, bool>> predicate, CancellationToken cancellationToken = default) => await Task.FromResult(context.Plans.Where(predicate).AsEnumerable());



    public async Task<PaginationResult<ViewMultiplePlanWidget>> GetMultiplePlanWidgetDataAsync(ViewPlanWidgetRequest req)
    {
        SqlBuilder sb = new SqlBuilder(@"
            SELECT
                pn.PlanID,
                pn.PlanName,
                pn.TenantID,
                pn.TodoCount,
                pn.TotalCount,
                pn.PendingWriteCount,
                pn.PendingReviewCount,
                pn.HasViewPermission,
                pn.HasEditPermission,
                pn.Show
            FROM [dbo].[fnGetPlanWithPermission](@Responsible) pn")
            .WhereScope(where =>
            {
                where.AddWhereClause(" AND pn.TenantID = @TenantID");
                where.AddWhereClause(" AND pn.HasViewPermission = 1");
                where.AddWhereClauseByCondition(" AND pn.Show = 1 ", !req.ShowHiddenData);
                where.AddWhereClauseByCondition(" AND (pn.PendingWriteCount <> 0 OR pn.PendingReviewCount <> 0 )", true);
            })
            .AddOrderBy("pn.PlanID")
            .AddPagination(req.Page, req.PerPage);

        return await PaginationResultBuilder<ViewMultiplePlanWidget>(context, sb.ToString(), req);
    }

    public async Task<PaginationResult<ViewSinglePlanWidget>> GetPlanWidgetDataAsync(ViewPlanWidgetRequest req)
    {
        SqlBuilder sb = new SqlBuilder(@"
            SELECT
                p.PlanID,
                p.PlanName,
                p.TenantID,
                p.TotalCount,
                p.SingleWidgetPendingWriteCount,
                p.ShouldOpenCount,
                p.PendingReviewCount,
                p.SendBackCount,
                p.CanceledCount,
                p.ApprovedCount,
                p.RejectedCount,
                p.UnOpenCount,
                p.HasViewPermission,
                p.HasEditPermission,
                p.Show
            FROM [dbo].[fnGetPlanWithPermission](@Responsible) p")
            .WhereScope(where =>
            {
                where.AddWhereClause(" AND p.TenantID = @TenantID");
                where.AddWhereClause(" AND p.HasViewPermission = 1");
                where.AddWhereClauseByCondition(" AND p.Show = 1 ", !req.ShowHiddenData);
                where.AddWhereClauseByCondition(" AND p.SingleWidgetPendingWriteCount = 0 ", req.IsCompleted ?? false);
                where.AddWhereClauseByCondition(" AND p.SingleWidgetPendingWriteCount > 0 ", req.IsCompleted.HasValue && !req.IsCompleted.Value);
            })
            .AddOrderBy("p.PlanID")
            .AddPagination(req.Page, req.PerPage);

        return await PaginationResultBuilder<ViewSinglePlanWidget>(context, sb.ToString(), req);
    }

    public async Task<PaginationResult<ViewPlan>> ListDtoAsync(QueryPlanRequest req, CancellationToken cancellationToken = default)
    {
        SqlBuilder sb = new SqlBuilder(@"
            SELECT
                pn.PlanID,
                pn.PlanName,
                pn.Year,
                pn.CompanyID,
                pn.TenantID,
                pn.FactoryIdList,
                pn.IndicatorIdList,
                pn.CustomIndicatorIdList,
                pn.IndustryIdList,
                pn.TodoCount,
                pn.TotalCount,
                pn.PendingWriteCount,
                pn.PendingReviewCount,
                pn.Show,
                pn.CreatedDate,
                pn.CreatedUser,
                pn.HasSettingPermission,
                pn.HasViewPermission,
                pn.HasEditPermission,
                pn.ModifiedDate,
                pn.ModifiedUser
            FROM [dbo].[fnGetPlanWithPermission](@Responsible) pn")
            .WhereScope(where =>
            {
                where.AddWhereClause(" AND pn.TenantID = @TenantID");
                where.AddWhereClause(" AND pn.HasViewPermission = 1");
                where.AddWhereClauseByCondition(" AND pn.Show = 1 ", !req.ShowHiddenData);
                where.AddWhereClauseByCondition(" AND pn.PendingWriteCount = 0 AND pn.PendingReviewCount = 0 ", req.IsCompleted ?? false);
                where.AddWhereClauseByCondition(" AND (pn.PendingWriteCount <> 0 OR pn.PendingReviewCount <> 0 )", req.IsCompleted.HasValue && !req.IsCompleted.Value);
                where.AddWhereClauseByCondition(" AND pn.PlanName COLLATE SQL_Latin1_General_CP1_CI_AS LIKE '%' + @PlanName + '%' ", !req.PlanName.IsNullOrEmpty());
            })
            .AddOrderBy(req.SortOrder ?? "ModifiedDate DESC")
        .AddPagination(req.Page, req.PerPage);

        return await PaginationResultBuilder<ViewPlan>(context, sb.ToString(), req);
    }

    public async Task<IEnumerable<ViewPlanExportDataSet>> GetExportDataSetAsync(string[] planDetailIds, CancellationToken cancellationToken = default)
    {

        SqlBuilder sb = new SqlBuilder(@"
            SELECT
	            c.PlanDetailName,
	            c.CycleType,
	            b.Month,
	            b.Year,
	            b.Quarter,
	            a.FieldID,
	            a.FieldName,
	            a.FieldValue
            FROM PlanDocumentData a
            LEFT JOIN PlanDocument b ON a.PlanDocumentID = b.PlanDocumentID
            LEFT JOIN PlanDetail c ON c.PlanDetailID = b.PlanDetailID
        ")
        .WhereScope(where =>
        {
            where.AddWhereClause(" AND a.Archived = 0");
            where.AddWhereClause(" AND b.PlanDetailID IN @planDetailIds");
        })
        .AddOrderBy("c.PlanDetailID, b.Year, b.Month, b.Quarter");

        return await context.QueryAsync<ViewPlanExportDataSet>(sb.ToString(), new { planDetailIds });
    }

    public async Task<IEnumerable<string>> GetYearListAsync(string tenantId, CancellationToken cancellationToken = default) => await context.Plans
        .AsNoTracking().Where(x => x.TenantId == tenantId).Select(x => x.Year)
        .Distinct()
        .OrderBy(year => year)
        .ToListAsync(cancellationToken);
}