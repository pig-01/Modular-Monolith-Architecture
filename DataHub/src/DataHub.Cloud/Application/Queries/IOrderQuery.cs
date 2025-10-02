using DataHub.Domain.AggregatesModel.OrderAggregate;
using DataHub.Domain.SeedWork;

namespace DataHub.Cloud.Application.Queries;

public interface IOrderQuery : IQuery<Order>
{
    /// <summary>
    /// 取得訂單資料
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="pricingCode"></param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<Order?> GetUserOrders(string email, string pricingCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 是否已經有試用該方案
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="sku">產品分類</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<bool> HasProvision(string email, string sku, CancellationToken cancellationToken = default);
}
