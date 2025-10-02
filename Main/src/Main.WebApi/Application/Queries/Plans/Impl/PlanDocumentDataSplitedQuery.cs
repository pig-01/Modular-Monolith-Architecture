using System.Linq.Expressions;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Utilities.SqlBuilder;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanDocumentDataSplitedQuery(DemoContext context, ITimeZoneService timeZoneService) : IPlanDocumentDataSplitedQuery
{

    public async Task<IEnumerable<int>> GetYearRangeAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        SqlBuilder sb = new SqlBuilder(@"
            SELECT DISTINCT YearValue
            FROM (
                SELECT YEAR(StartDate) AS YearValue FROM PlanDocumentDataSplited WHERE Archived = 0 AND TenantID = @tenantId
                UNION ALL
                SELECT YEAR(EndDate) FROM PlanDocumentDataSplited WHERE Archived = 0 AND TenantID = @tenantId
            ) AS CombinedYears
            ORDER BY YearValue;
        ");
        return await context.QueryAsync<int>(sb.ToString(), new { tenantId });
    }

    public async Task<IEnumerable<ViewPlanDocumentDataSplited>> GetDtoByDashboardArgsAsync(QueryPlanDocumentDataSplitedRequest req, CancellationToken cancellationToken = default)
    {

        SqlBuilder sb = new SqlBuilder(@"
            SELECT
                d.[Year],
                b.[Month],
	            b.[Quarter],
                a.*
            FROM PlanDocumentDataSplited a
            LEFT JOIN PlanDocument b ON a.PlanDocumentID = b.PlanDocumentID
            LEFT JOIN PlanDetail c ON c.PlanDetailID = b.PlanDetailID
            LEFT JOIN [Plan] d ON d.PlanID = c.PlanID
        ")
        .WhereScope(where =>
        {
            where.AddWhereClause("AND b.PlanDocumentID IS NOT NULL");
            where.AddWhereClause("AND a.Archived = 0");
            where.AddWhereClause("AND a.TenantID = @TenantID");
            where.AddWhereClause("AND a.StartDate >= @StartYear");
            where.AddWhereClause("AND a.EndDate <= @EndYear");
            where.AddWhereClauseByCondition("AND ',' +  a.CustomName + ',' LIKE ',' + @CustomNameFields + ','", !String.IsNullOrEmpty(req.CustomNameFields));
            where.AddWhereClauseByCondition("AND ',' +  a.CompanyName + ',' LIKE ',' + @CompanyNameList  + ','", !String.IsNullOrEmpty(req.CompanyNameList));
            where.AddWhereClauseByCondition("AND ',' +  a.AreaName + ',' LIKE ',' + @AreaNameList  + ','", !String.IsNullOrEmpty(req.AreaNameList));
        });

        req.StartYear = timeZoneService.ConvertToUtc(req.StartYear);
        req.EndYear = timeZoneService.ConvertToUtc(req.EndYear);

        return await context.QueryAsync<ViewPlanDocumentDataSplited>(sb.ToString(), req);
    }

    public async Task<IEnumerable<ViewPlanDocumentDataSplited>> GetDtoByPlanDocumentIdAsync(int planDocumentId, CancellationToken cancellationToken = default)
    {

        SqlBuilder sb = new SqlBuilder(@"
            SELECT
                d.[Year],
                b.[Month],
	            b.[Quarter],
                a.*
            FROM PlanDocumentDataSplited a
            LEFT JOIN PlanDocument b ON a.PlanDocumentID = b.PlanDocumentID
            LEFT JOIN PlanDetail c ON c.PlanDetailID = b.PlanDetailID
            LEFT JOIN [Plan] d ON d.PlanID = c.PlanID
        ")
        .WhereScope(where =>
        {
            where.AddWhereClause("AND b.PlanDocumentID = @planDocumentId");
            where.AddWhereClause("AND a.Archived = 0");

        });

        return await context.QueryAsync<ViewPlanDocumentDataSplited>(sb.ToString(), new { planDocumentId });
    }

    public async Task<IEnumerable<ViewPlanDocumentDataSplitConfig>> GetConfigListDtoAsync(CancellationToken cancellationToken = default)
    {
        SqlBuilder sb = new(@"
            SELECT
                [ID]
                ,[FieldID]
                ,[Description]
                ,[IsEnabled]
                ,[CreatedDate]
                ,[CreatedUser]
                ,[ModifiedDate]
                ,[ModifiedUser]
                ,[FieldName]
            FROM PlanDocumentDataSplitConfig
            WHERE IsEnabled = 1
        ");
        return await context.QueryAsync<ViewPlanDocumentDataSplitConfig>(sb.ToString(), new { });
    }

    public Task<PlanDocumentDataSplited?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    Task<IEnumerable<PlanDocumentDataSplited>> IQuery<PlanDocumentDataSplited>.ListAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

    Task<IEnumerable<PlanDocumentDataSplited>> IQuery<PlanDocumentDataSplited>.ListAsync(Expression<Func<PlanDocumentDataSplited, bool>> predicate, CancellationToken cancellationToken) => throw new NotImplementedException();
}
