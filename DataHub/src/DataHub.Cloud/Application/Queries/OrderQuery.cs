using System.Linq.Expressions;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using DataHub.Infrastructure.Contexts;

namespace DataHub.Cloud.Application.Queries;

public class OrderQuery(DemoContext context) : IOrderQuery
{
    public Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotImplementedException();


    /// <summary>
    /// 取得訂單資料
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="pricingCode"></param>
    /// <param name="cancellationToken">取消憑證</param>
    public async Task<Order?> GetUserOrders(string email, string pricingCode, CancellationToken cancellationToken = default)
    {
        string sql = @"
            SELECT a.OrderId, a.SDate, a.EDate 
            FROM Orders a
            JOIN CustomerOrdersRelation b ON b.OrderId = a.OrderId
            JOIN CustomerSCUserRelation c ON c.CustomerId = b.CustomerId
            WHERE c.UserID = @UserID
                AND a.PricingCode = @PricingCode ";

        return await context.QueryFirstOrDefaultAsync<Order>(sql, new { UserID = email, PricingCode = pricingCode });
    }

    /// <summary>
    /// 是否已經有試用該方案
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="sku">產品分類</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    public async Task<bool> HasProvision(string email, string sku, CancellationToken cancellationToken = default)
    {
        string sql = @"
            SELECT COUNT(*)
            from CustomerSCUserRelation c 
            inner join CustomerOrdersRelation a on c.CustomerId = a.CustomerId
            inner join Orders b on a.OrderId = b.OrderId
            WHERE c.UserID = @email and b.PricingCode = @sku ";

        return await context.QueryFirstAsync<int>(sql, new { email, sku }) > 0;
    }
    public Task<IEnumerable<Order>> ListAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<IEnumerable<Order>> ListAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
