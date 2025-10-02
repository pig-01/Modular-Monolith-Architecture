using System.Linq.Expressions;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Utilities.SqlBuilder;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Dto.ViewModel.Plan;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanDetailQuery(DemoContext context) : BaseQuery, IPlanDetailQuery
{
    private readonly DemoContext context = context;

    public async Task<PlanDetail?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => await context.PlanDetails
            .FirstOrDefaultAsync(x => x.PlanDetailId == id, cancellationToken);

    public async Task<IEnumerable<PlanDetail>> GetByIdListAsync(int[] idList, CancellationToken cancellationToken = default) => await context.PlanDetails
            .Where(x => idList.Contains(x.PlanDetailId))
            .ToListAsync(cancellationToken);


    public async Task<ViewPlanDetail?> GetDtoByIdAsync(int id, string responsible, CancellationToken cancellationToken = default)
    {
        Dictionary<int, ViewPlanDetail> planDictionary = [];
        _ = await context.QueryAsync<ViewPlanDetail, ViewPlanDocument>(@"
            SELECT
	            pd.PlanDetailID,
	            pd.PlanID,
	            pd.PlanDetailName,
	            pd.PlanDetailEnName,
	            pd.PlanDetailChName,
	            pd.PlanDetailJpName,
	            pd.PlanTemplateID,
	            pd.FormID,
		        pd.AcceptDataSource,
		        pd.NetzeroReportId,
                pd.NetzeroReportName,
		        pd.ApiConnectionId,
                pd.ApiConnectionFailed,
                pd.ResponsibleList,
                pd.ResponsibleNameList,
	            pd.GroupID,
	            pd.I18nGroupName,
	            pd.Show,
	            pd.EndDate,
                pd.RowNumber,
                pd.CycleType,
                pd.CycleMonth,
                pd.CycleDay,
                pd.CycleMonthLast,
	            pn.Year,
	            pd.CreatedDate,
	            pd.CreatedUser,
	            pd.ModifiedDate,
	            pd.ModifiedUser,
                pp.HasViewPermission,
                pp.HasEditPermission,
	            pdc.PlanDetailID,
                pdc.PlanFactoryID,
	            pdc.PlanDocumentID,
	            pdc.DocumentID,
	            pdc.Responsible,
	            pdc.ResponsibleName,
	            pdc.Approve,
	            pdc.ApproveName,
	            pdc.FormStatus,
	            pdc.I18nFormStatusName,
	            pdc.FormStatusColor,
	            pdc.StartDate,
	            pdc.StartDate,
	            pdc.EndDate,
	            pdc.Year,
	            pdc.Quarter,
	            pdc.Month,
	            pdc.CycleType,
	            pdc.CreatedDate,
	            pdc.CreatedUser,
	            pdc.ModifiedDate,
	            pdc.ModifiedUser
            FROM [vwPlanDetail] pd WITH(NOLOCK)
            JOIN [Plan] pn WITH(NOLOCK) ON pn.PlanID = pd.PlanID
            LEFT JOIN dbo.fnGetPlanPermissions(@Responsible) pp ON pn.PlanID = pp.PlanID
            LEFT JOIN [vwPlanDocument] pdc WITH(NOLOCK) ON  pd.PlanDetailID = pdc.PlanDetailID
            WHERE pd.PlanDetailID = @PlanDetailId AND pp.HasViewPermission = 1
", (planDetail, planDocument) =>
        {
            if (!planDictionary.TryGetValue(planDetail.PlanDetailId, out ViewPlanDetail? currentPlanDetail))
            {
                currentPlanDetail = planDetail;
                planDictionary.Add(planDetail.PlanDetailId, currentPlanDetail);
            }

            if (planDocument != null)
                currentPlanDetail.PlanDocumentList.Add(planDocument);

            return currentPlanDetail;
        }, new { PlanDetailId = id, Responsible = responsible }, splitOn: "PlanDetailID");

        return planDictionary.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<ViewPlanDetail>> GetDtoByPlanIdAsync(int planId, string responsible, CancellationToken cancellationToken = default)
    {
        Dictionary<int, ViewPlanDetail> planDictionary = [];
        _ = await context.QueryAsync<ViewPlanDetail, ViewPlanDocument>(@"
        SELECT
            pd.PlanDetailID,
            pd.PlanID,
            pd.PlanDetailName,
            pd.PlanDetailChName,
            pd.PlanDetailEnName,
            pd.PlanDetailJpName,
            pd.PlanTemplateID,
            pd.CustomPlanTemplateID,
            pd.FormID,
		    pd.AcceptDataSource,
		    pd.NetzeroReportId,
		    pd.ApiConnectionId,
            pd.ApiConnectionFailed,
            pd.ResponsibleList,
            pd.ResponsibleNameList,
            pd.GroupID,
            pd.I18nGroupName,
            pd.Show,
            pd.EndDate,
            pd.RowNumber,
            pd.ShowHint,
            pd.CycleType,
            pd.I18nCycleTypeName,
            pd.CycleMonth,
            pd.CycleDay,
            pd.CycleMonthLast,
            pn.Year,
            pd.CreatedDate,
            pd.CreatedUser,
            pd.ModifiedDate,
            pd.ModifiedUser,
            pp.HasViewPermission,
            pp.HasEditPermission,
            pdc.PlanDetailID,
            pdc.PlanDocumentID,
            pdc.PlanFactoryID,
            pdc.DocumentID,
            pdc.Responsible,
            pdc.ResponsibleName,
            pdc.Approve,
            pdc.ApproveName,
            pdc.FormStatus,
            pdc.I18nFormStatusName,
            pdc.FormStatusColor,
            pdc.StartDate,
            pdc.StartDate,
            pdc.EndDate,
            pdc.Year,
            pdc.Quarter,
            pdc.Month,
            pdc.CycleType,
            pdc.CreatedDate,
            pdc.CreatedUser,
            pdc.ModifiedDate,
            pdc.ModifiedUser
        FROM [vwPlanDetail] pd
        JOIN [Plan] pn ON pn.PlanID = @PlanId AND pn.PlanID = pd.PlanID
        LEFT JOIN dbo.fnGetPlanPermissions(@Responsible) pp ON pn.PlanID = pp.PlanID
        LEFT JOIN [vwPlanDocument] pdc ON  pd.PlanDetailID = pdc.PlanDetailID
        WHERE pp.HasViewPermission = 1
        ORDER BY pd.PlanDetailID
        ", (planDetail, planDocument) =>
        {
            if (!planDictionary.TryGetValue(planDetail.PlanDetailId, out ViewPlanDetail? currentPlanDetail))
            {
                currentPlanDetail = planDetail;
                planDictionary.Add(planDetail.PlanDetailId, currentPlanDetail);
            }

            if (planDocument != null)
                currentPlanDetail.PlanDocumentList.Add(planDocument);

            return currentPlanDetail;
        }, new { PlanId = planId, Responsible = responsible }, splitOn: "PlanDetailID");

        return planDictionary.Values;
    }

    public async Task<IEnumerable<PlanDetail>> ListAsync(CancellationToken cancellationToken = default) => await context.PlanDetails
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlanDetail>> ListAsync(Expression<Func<PlanDetail, bool>> predicate, CancellationToken cancellationToken = default) => await context.PlanDetails
            .Where(predicate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlanDetail>> ListByPlanIdAsync(int planId, CancellationToken cancellationToken = default) => await context.PlanDetails
            .Include(x => x.PlanDocuments)
            .Where(x => x.PlanId == planId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ViewPlanDetail>> ListDtoAsync(QueryPlanDetailRequest request, CancellationToken cancellationToken = default)
    {

        SqlBuilder sb = new SqlBuilder(@"
           SELECT
	            pd.PlanDetailID,
	            pd.PlanID,
	            pd.PlanDetailName,
	            pd.FormID,
                pd.DataSource,
                pd.AcceptDataSource,
                pd.ApiConntectionId,
                pd.NetzeroReportId,
                pd.NetzeroReportName,
	            pd.GroupID,
	            sd.Name as GroupName,
                pd.RowNumber,
                pd.CycleType,
                pd.CycleMonth,
                pd.CycleDay,
                pd.CycleMonthLast,
	            pd.Show,
	            pd.EndDate,
	            pd.CreatedDate,
	            pd.CreatedUser,
	            pd.ModifiedDate,
	            pd.ModifiedUser
            FROM PlanDetail pd
            LEFT JOIN SystemData sd ON sd.SystemCode = pd.GroupID AND sd.CodeType = 'DemoFormStatus'

        ")
          .WhereScope(where =>
          {
              where.AddWhereClauseByCondition(" AND pd.PlanID = @PlanID", request.PlanID.HasValue);
              where.AddWhereClauseByCondition(" AND pd.PlanDetailName = @PlanDetailName", !request.PlanDetailName.IsNullOrEmpty());
          });

        return await context.QueryAsync<ViewPlanDetail>(sb.ToString(), request);
    }
}
