using System.Threading;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.TimeZone;
using DataHub.Cloud.Models.Provision;
using DataHub.Domain.AggregatesModel.CustomerAggregate;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using DataHub.Domain.AggregatesModel.TenantAggregate;
using DataHub.Domain.AggregatesModel.UserAggregate;
using DataHub.Domain.Events;
using DataHub.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataHub.Cloud.Repositories;

public class ProvisionRepository(DemoContext context) : IProvisionRepository
{
    /// <summary>
    /// 檢查帳號是否已經存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns></returns>
    public async Task<bool> IsSCUserExists(string email) =>
        await context.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(1) FROM SCUser a WHERE UserID = @email",
            new { email }) > 0;

    /// <summary>
    /// 是否已經有試用該方案
    /// </summary>
    /// <param name="customerId">CustomerId</param>
    /// <returns></returns>
    public virtual async Task<bool> IsAlreadyTrail(string customerId)
    {
        string sql = @"
                    SELECT COUNT(1)
                    FROM CustomerOrdersRelation a
                    inner join Orders b on a.OrderId = b.OrderId
                    WHERE a.CustomerId = @customerId ";

        return await context.QueryFirstAsync<int>(sql, new { customerId }) > 0;
    }


    /// <summary>
    /// 取得方案
    /// </summary>
    /// <param name="pricingCode">資費方案代號</param>
    /// <returns></returns>
    public async Task<Pricing?> GetPricing(string pricingCode)
    {
        string sql = @"
            SELECT * FROM Pricing a WHERE a.PricingCode = @pricingCode;
            SELECT * FROM PricingDetail a WHERE a.PricingCode = @pricingCode;";

        using SqlMapper.GridReader reader = await context.QueryMultipleAsync(sql, new { pricingCode });

        Pricing? pricing = reader.Read<Pricing>().FirstOrDefault();
        if (pricing != null) pricing.PricingDetails = [.. reader.Read<PricingDetail>()];
        return pricing;
    }

    /// <summary>
    /// 寫入Customer
    /// </summary>
    /// <param name="customerId">CustomerId</param>
    public async Task UpsertCustomer(string customerId)
    {
        string sql = @"
            IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerId = @customerId)
            BEGIN
                INSERT INTO Customer (CustomerId) VALUES (@customerId)
            END";

        await context.ExecuteAsync(sql, new { customerId });
    }

    /// <summary>
    /// 寫入Customer跟SCUser關聯
    /// </summary>
    /// <param name="customerId">CustomerID</param>
    /// <param name="email">電子郵件</param>
    public async Task UpsertCustomerSCUserRelation(string customerId, string email)
    {
        string sql = @"
            IF NOT EXISTS (SELECT 1 FROM CustomerSCUserRelation WHERE CustomerId = @customerId AND UserID = @email)
            BEGIN
                INSERT INTO CustomerSCUserRelation (CustomerId, UserID) VALUES (@customerId, @email)
            END";

        await context.ExecuteAsync(sql, new { customerId, email });
    }


    /// <summary>
    /// 寫入訂單資訊
    /// </summary>
    /// <param name="customerId">CustomerID</param>
    /// <param name="pricingCode">資費方案代號</param>
    /// <param name="orderDate">下單時間</param>
    /// <param name="startDate">訂單開始日</param>
    /// <param name="endDate">訂單結束日</param>
    public async Task<Order> InsertOrders(string customerId, string pricingCode, DateTime orderDate, DateTime startDate, DateTime endDate)
    {
        EntityEntry<Order> order = await context.Orders.AddAsync(Order.Create(pricingCode, orderDate, startDate, endDate));
        _ = await context.SaveChangesAsync(); // 儲存變更以確保 OrderId 已經生成
        long orderId = order.Entity.OrderId; // 取得新增的訂單ID

        // 新增訂單明細，從價格明細中複製
        await context.OrderDetails.AddRangeAsync(
            context.PricingDetails
                .Where(pd => pd.PricingCode == pricingCode)
                .Select(pd => new OrderDetail
                {
                    OrderId = orderId,
                    PricingDetailCode = pd.PricingDetailCode,
                    PricingDetailValue = pd.PricingDetailValue
                }));

        // 新增客戶訂單關聯
        _ = await context.CustomerOrdersRelations.AddAsync(new CustomerOrdersRelation
        {
            CustomerId = customerId,
            OrderId = orderId
        });

        return order.Entity;
    }

    /// <summary>
    /// 更新帳號可管理最大Tenant數
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="maxTenantNo">最大Tenant數</param>
    public async Task UpdateSCUserMaxTenantNo(string email, int maxTenantNo) =>
        await context.ExecuteAsync(
            "UPDATE SCUser SET MaxTenantNo = @maxTenantNo WHERE UserID = @email",
            new
            {
                email,
                maxTenantNo
            });


    /// <summary>
    /// 寫入帳號資訊
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <param name="maxTenantNo">最大Tenant數</param>
    public async Task CreateSCUser(string email, int maxTenantNo)
    {
        string sql = @"
            INSERT INTO SCUser (UserID, UserName, PasswordHash, UserType, UserDesc, UserEmail, MaxTenantNo, LastSuccessfulLoginDate, LastFailedLoginDate, RepeatedFailloginTimes, Culture, CreatedDate, CreatedUser, ModifiedDate, ModifiedUser)
            VALUES (@UserID, @UserName, @PasswordHash, @UserType, @UserDesc, @UserEmail, @MaxTenantNo, null, null, 0, @Culture, @CreatedDate, @CreatedUser, @ModifiedDate, @ModifiedUser);
            UPDATE SCUser SET PasswordHash = sys.fn_varbintohexsubstring(0, hashbytes('SHA1', UserID + ':' + UserID), 1, 0) WHERE UserID = @UserID;
        ";

        await context.ExecuteAsync(sql, new
        {
            UserID = email,
            UserName = email,
            PasswordHash = "",
            UserType = "",
            UserDesc = "",
            UserEmail = email,
            MaxTenantNo = maxTenantNo,
            Culture = "zh-CHT",  // 預設給中文
            CreatedDate = DateTimeOffset.Now,
            CreatedUser = "系統管理員(SuperUser)(API)",
            ModifiedDate = DateTimeOffset.Now,
            ModifiedUser = "系統管理員(SuperUser)(API)"
        });
    }

    public async Task<SequenceNo?> GetTenantID() => await GetNumber("TenantID", "T", 9, 1);

    public async Task<SequenceNo?> GetNumber(string orderType, string prefix, int seqLength, int howMany) => await context.QueryFirstOrDefaultAsync<SequenceNo>("spGetSeqNo", new
    {
        type = orderType,
        prefix,
        leng = seqLength,
        count = howMany
    });

    public async Task<(string tenantId, long userTenantId)> CreateTenant(string name, long orderId, string accoountId, DateOnly applicationDate, DateTime startDate, DateTime endDate, byte[] logo)
    {
        // 要號並建立站台
        string tenantId = (await GetTenantID())?.NumberFrom ?? throw new InvalidOperationException("Failed to get Tenant ID");
        EntityEntry<Tenant> tenant = await context.Tenants.AddAsync(Tenant.Create(tenantId, name, accoountId, "", logo, applicationDate, startDate, endDate));

        // 建立站台第一位使用者為供裝帳號
        EntityEntry<ScuserTenant> scuserTenant = await context.ScuserTenants.AddAsync(new ScuserTenant(tenantId, accoountId));

        // 建立站台與訂單關聯
        _ = await context.OrderTenants.AddAsync(new OrderTenant(tenantId, orderId));

        // 建立或調整站台與使用者關聯
        CurrentTenant? currentTenant = await context.CurrentTenants.FirstOrDefaultAsync(x => x.UserId == accoountId);
        if (currentTenant is not null)
        {
            _ = context.CurrentTenants.Remove(currentTenant);
        }
        _ = await context.CurrentTenants.AddAsync(CurrentTenant.Create(tenantId, accoountId));

        // 發送站台建立完成的通知郵件
        tenant.Entity.AddDomainEvent(new SendProvisionMailDomainEvent(tenant.Entity));

        // 儲存變更
        _ = await context.SaveChangesAsync();
        return (tenant.Entity.TenantId, scuserTenant.Entity.UserTenantId);
    }

    /// <summary>
    /// 建立對應定價方案角色進入產品角色
    /// 必須為有站台的人員
    /// </summary>
    /// <param name="pricingCode">定價方案</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="userTenantId">站台帳號識別碼</param>
    /// <param name="userId">帳號識別碼</param>
    /// <returns></returns>
    public async Task AssignPricingRoleToProduct(string pricingCode, string tenantId, long userTenantId, string userId)
    {
        // 取得定價方案角色資訊
        IEnumerable<PricingRole> pricingRoles = await context.PricingRoles.AsNoTracking().Where(x => x.PricingCode == pricingCode).ToListAsync() ?? throw new NotFoundException("定價方案不存在");

        // 依照設定的角色代碼查出角色權限範本
        foreach (PricingRole pricingRole in pricingRoles)
        {
            // 檢查是否有權限範本
            Scrole? template = await context.Scroles.AsNoTracking()
                .Include(x => x.ScroleFunctions)
                .FirstOrDefaultAsync(x => x.RoleCode == pricingRole.RoleCode && x.TenantId == "SuperTenant");
            if (template is null) continue; // 如果角色範本不存在，則跳過建立

            // 建立依照範本對應的功能給新站台建立角色
            EntityEntry<Scrole> scrole = await context.Scroles.AddAsync(Scrole.Create(pricingRole.RoleCode, pricingRole.RoleName, tenantId, userId));
            _ = await context.SaveChangesAsync();

            if (template.IsManager)
            {
                // 如果是管理員角色，則建立站台管理員角色
                _ = await context.ScuserRoles.AddAsync(ScuserRole.Create(scrole.Entity.RoleId, userTenantId, userId));
            }

            // 建立角色對應的功能
            await context.ScroleFunctions.AddRangeAsync(template.ScroleFunctions.Select(x => new ScroleFunction()
            {
                RoleId = scrole.Entity.RoleId,
                FunctionId = x.FunctionId,
                CreatedUser = userId
            }));
            _ = await context.SaveChangesAsync();
        }
    }
}
