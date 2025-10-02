using System.Linq.Expressions;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Dto.ViewModel.Plan;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.Plans.Impl;

public class PlanDocumentQuery(DemoContext context) : IPlanDocumentQuery
{
    /// <summary>
    /// 取得表單文件，根據指定的計畫明細 ID 和月份
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="month">月份</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    public async Task<PlanDocument?> GetByDetailIdAndMonthAsync(int detailId, int month, CancellationToken cancellationToken = default)
        => await context.PlanDocuments.AsNoTracking().FirstOrDefaultAsync(
            x => x.PlanDetailId == detailId && x.Month == month,
            cancellationToken);

    /// <summary>
    /// 取得表單文件，根據指定的計畫明細 ID 和季度
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="quarter">季度</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    public async Task<PlanDocument?> GetByDetailIdAndQuarterAsync(int detailId, int quarter, CancellationToken cancellationToken = default)
        => await context.PlanDocuments.AsNoTracking().FirstOrDefaultAsync(
            x => x.PlanDetailId == detailId && x.Quarter == quarter,
            cancellationToken);

    /// <summary>
    /// 取得表單文件，根據指定的計畫明細 ID 和年度
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="year">年度</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    public async Task<PlanDocument?> GetByDetailIdAndYearAsync(int detailId, int year, CancellationToken cancellationToken = default)
        => await context.PlanDocuments.AsNoTracking().FirstOrDefaultAsync(
            x => x.PlanDetailId == detailId && x.Year == year,
            cancellationToken);

    /// <summary>
    /// 取得指定 ID 的計畫文件
    /// </summary>
    /// <param name="id">文件 ID</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    public async Task<PlanDocument?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await context.PlanDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PlanDocumentId == id, cancellationToken);

    /// <summary>
    /// 取得指定文件 ID 的計畫文件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PlanDocument?> GetByDocumentIdAsync(long id, CancellationToken cancellationToken = default)
        => await context.PlanDocuments
            .Include(x => x.PlanDocumentDatas)
            .Include(x => x.PlanDetail)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DocumentId == id, cancellationToken);

    /// <summary>
    /// 列出指定計畫明細 ID 的所有計畫文件
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    public async Task<IEnumerable<PlanDocument>> ListByDetailIdAsync(int detailId, CancellationToken cancellationToken = default)
        => await context.PlanDocuments
            .AsNoTracking()
            .Where(x => x.PlanDetailId == detailId)
            .ToListAsync(cancellationToken);

    public async Task<Dictionary<string, object>> GetPlanDocumentDataForExcel(string planDtailIdList, string fieldIdList, CancellationToken cancellationToken = default)
    {
        dynamic? result = await context.QueryFirstOrDefaultAsync<dynamic>(@"
            EXEC [spGetPlanDocumentDataForExcelByFieldID] @PlanDtailIdList, @FieldIdList
        ", new { PlanDtailIdList = planDtailIdList, FieldIdList = fieldIdList });

        Dictionary<string, object> dict = [];

        if (result == null)
            return dict;

        // dynamic 轉 Dictionary<string, object>
        foreach (KeyValuePair<string, object> prop in (IDictionary<string, object>)result)
        {
            dict[prop.Key] = prop.Value;
        }

        return dict;
    }

    public Task<ViewPlanDocument?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IEnumerable<PlanDocument>> ListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<IEnumerable<PlanDocument>> ListAsync(Expression<Func<PlanDocument, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
