using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.SeedWork;

namespace Main.WebApi.Application.Queries.CustomTemplate;

public interface ICustomPlanTemplateQuery : IQuery<CustomPlanTemplate>
{
    /// <summary>
    /// 取得指定要求單位的最新版本的自訂指標計畫套版
    /// </summary>
    /// <param name="requestUnitId">自訂要求單位識別碼</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<IEnumerable<CustomPlanTemplate?>> GetLastVersionAsync(long requestUnitId, string tenantId, CancellationToken cancellationToken = default);
}
