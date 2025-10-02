using DataHub.Cloud.Models;
using DataHub.Cloud.Models.Provision;
using DataHub.Domain.AggregatesModel.OrderAggregate;

namespace DataHub.Cloud.Repositories;

public interface IProvisionRepository
{
    /// <summary>
    /// 檢查帳號是否已經存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns></returns>
    Task<bool> IsSCUserExists(string email);

    /// <summary>
    /// 是否已經有試用該方案
    /// </summary>
    /// <param name="customerId">CustomerId</param>
    /// <returns></returns>
    Task<bool> IsAlreadyTrail(string customerId);

    /// <summary>
    /// 取得方案
    /// </summary>
    /// <param name="pricingCode">資費方案代號</param>
    /// <returns></returns>
    Task<Pricing?> GetPricing(string pricingCode);

    /// <summary>
    /// 寫入Customer
    /// </summary>
    /// <param name="customerId">CustomerId</param>
    Task UpsertCustomer(string customerId);

    /// <summary>
    /// 寫入Customer跟SCUser關聯
    /// </summary>
    /// <param name="customerId">CustomerID</param>
    /// <param name="email">電子郵件</param>
    Task UpsertCustomerSCUserRelation(string customerId, string email);

    /// <summary>
    /// 寫入訂單資訊
    /// </summary>
    /// <param name="customerId">CustomerID</param>
    /// <param name="pricingCode">資費方案代號</param>
    /// <param name="orderDate">下單時間</param>
    /// <param name="startDate">訂單開始日</param>
    /// <param name="endDate">訂單結束日</param>
    Task<Order> InsertOrders(string customerId, string pricingCode, DateTime orderDate, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 更新帳號可管理最大Tenant數
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="maxTenantNo">最大Tenant數</param>
    Task UpdateSCUserMaxTenantNo(string email, int maxTenantNo);

    /// <summary>
    /// 寫入帳號資訊
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="maxTenantNo">最大Tenant數</param>
    Task CreateSCUser(string email, int maxTenantNo);

    /// <summary>
    /// 建立站台
    /// </summary>
    /// <param name="name"></param>
    /// <param name="orderId"></param>
    /// <param name="accoountId"></param>
    /// <param name="applicationDate"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="logo"></param>
    /// <returns></returns>
    Task<(string tenantId, long userTenantId)> CreateTenant(string name, long orderId, string accoountId, DateOnly applicationDate, DateTime startDate, DateTime endDate, byte[] logo);

    /// <summary>
    /// 取得站台識別碼
    /// </summary>
    /// <returns></returns>
    Task<SequenceNo?> GetTenantID();

    /// <summary>
    /// 取得序號
    /// </summary>
    /// <param name="orderType"></param>
    /// <param name="prefix"></param>
    /// <param name="seqLength"></param>
    /// <param name="howMany"></param>
    /// <returns></returns>
    Task<SequenceNo?> GetNumber(string orderType, string prefix, int seqLength, int howMany);

    /// <summary>
    /// 建立對應定價方案角色進入產品角色
    /// 必須為有站台的人員
    /// </summary>
    /// <param name="pricingCode">定價方案</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="userTenantId">站台帳號識別碼</param>
    /// <param name="userId">帳號識別碼</param>
    /// <returns></returns>
    Task AssignPricingRoleToProduct(string pricingCode, string tenantId, long userTenantId, string userId);
}
