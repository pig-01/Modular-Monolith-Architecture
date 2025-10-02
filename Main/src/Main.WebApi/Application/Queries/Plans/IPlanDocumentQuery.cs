using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.SeedWork;
using Main.Dto.ViewModel.Plan;

namespace Main.WebApi.Application.Queries.Plans;

public interface IPlanDocumentQuery : IQuery<PlanDocument>
{
    /// <summary>
    /// 取得指定文件 ID 的計畫文件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PlanDocument?> GetByDocumentIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得指定計畫細節 ID 和月份的計畫文件
    /// </summary>
    /// <param name="detailId"></param>
    /// <param name="month"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PlanDocument?> GetByDetailIdAndMonthAsync(int detailId, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得表單文件，根據指定的計畫明細 ID 和季度
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="quarter">季度</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    Task<PlanDocument?> GetByDetailIdAndQuarterAsync(int detailId, int quarter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得表單文件，根據指定的計畫明細 ID 和年度
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="year">年度</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    Task<PlanDocument?> GetByDetailIdAndYearAsync(int detailId, int year, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得指定 ID 的計畫文件
    /// </summary>
    /// <param name="id">文件 ID</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    Task<ViewPlanDocument?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出指定計畫明細 ID 的所有計畫文件
    /// </summary>
    /// <param name="detailId">計畫明細 ID</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns></returns>
    Task<IEnumerable<PlanDocument>> ListByDetailIdAsync(int detailId, CancellationToken cancellationToken = default);

    Task<Dictionary<string, object>> GetPlanDocumentDataForExcel(string planDtailIdList, string fieldIdList, CancellationToken cancellationToken = default);
}
