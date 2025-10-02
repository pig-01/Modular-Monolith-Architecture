using System.Globalization;
using System.Linq.Expressions;
using Scheduler.Domain.AggregateModel.MailAggregate;
using Scheduler.Domain.Enums;
using Scheduler.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using MailBase = Base.Domain.Models.Mail;

namespace Scheduler.Application.Queries.Mails;

public class MailQuery(DemoContext context) : IMailQuery
{
    /// <summary>
    /// 取得指定識別碼的實體
    /// </summary>
    /// <param name="id">信件佇列識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>發信佇列</returns>
    public async Task<MailQueue?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await context.MailQueues.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    /// <summary>
    /// 取得寄件者資訊
    /// </summary>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>寄件者資訊</returns>
    public async Task<MailSender> GetMailSender(string tenantId, CancellationToken cancellationToken = default) =>
        await context.MailSenders.AsNoTracking()
            .FirstOrDefaultAsync(x => new[] { tenantId, TenantEnum.SuperTenant.Name }.Contains(x.TenantId), cancellationToken)
            ?? throw new InvalidOperationException("MailSender not found for the specified tenant and party.");


    public async Task<IEnumerable<MailBase.MailServiceParameter>> GetMailServiceParameters(string tenantId)
    {
        string sql = @"
        SELECT A.*
        FROM (
            SELECT DISTINCT
                a.ServiceType,
                a.Domain,
                a.Account,
                a.Password,
                a.MailServiceParameterID,
                b.TenantID
            FROM MailServiceParameter a
            JOIN MailServiceRelation b ON (a.MailServiceParameterID = b.MailServiceParameterID)
            JOIN SystemCode sc ON sc.Code = a.ServiceType AND sc.CodeType = 'MailServiceType'
        ) A
        WHERE (A.TenantID = @tenantID OR A.TenantID = 'SuperTenant')"
        ;
        return await context.QueryAsync<MailBase.MailServiceParameter>(sql, new { tenantID = tenantId });
    }

    public async Task<IEnumerable<MailBase.MailServiceParameter>> GetMailServiceParameterByID(string mailServiceParameterID)
    {
        string sql = @"
            SELECT MailServiceParameterID, ServiceType, [Domain], Account, Password, TenantID, EnableSSL
            WHERE MailServiceParameterID = @mailServiceParameterID";

        return await context.QueryAsync<MailBase.MailServiceParameter>(sql, new { mailServiceParameterID });
    }
    public async Task<MailBase.MailTemplate?> GetMailTemplate(string functionCode, string mailType, string tenantId)
    {
        string sql = @"
            SELECT MailTemplateID, FunctionCode, MailType, zhCHTSubject, zhCHTBody, enUSSubject, enUSBody, zhCHSSubject, zhCHSBody, jaJPSubject, jaJPBody, TenantID, CreatedDate, CreatedUser, ModifiedDate, ModifiedUser, MailTemplateName
            FROM MailTemplate
            WHERE FunctionCode = @functionCode AND MailType = @mailType AND TenantID = @tenantId
            UNION ALL
            SELECT MailTemplateID, FunctionCode, MailType, zhCHTSubject, zhCHTBody, enUSSubject, enUSBody, zhCHSSubject, zhCHSBody, jaJPSubject, jaJPBody, TenantID, CreatedDate, CreatedUser, ModifiedDate, ModifiedUser, MailTemplateName
            FROM MailTemplate
            WHERE FunctionCode = @functionCode AND MailType = @mailType AND TenantID = 'SuperTenant'";

        return await context.QueryFirstOrDefaultAsync<MailBase.MailTemplate>(sql, new { functionCode, mailType, tenantId });
    }

    /// <summary>
    /// 取得待處理的發信佇列
    /// </summary>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>發信佇列清單</returns>
    public async Task<IEnumerable<MailQueue>> GetPendingMailItemsAsync(CancellationToken cancellationToken = default) =>
        await context.MailQueues.Where(x => x.Status == MailQueueStatus.Pending.Id.ToString(CultureInfo.InvariantCulture)).ToListAsync(cancellationToken);

    /// <summary>
    /// 取得所有發信佇列
    /// </summary>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>發信佇列清單</returns>
    public async Task<IEnumerable<MailQueue>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.MailQueues.ToListAsync(cancellationToken);

    /// <summary>
    /// 取得符合條件的發信佇列
    /// </summary>
    /// <param name="predicate">符合條件</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>發信佇列清單</returns>
    public async Task<IEnumerable<MailQueue>> ListAsync(Expression<Func<MailQueue, bool>> predicate, CancellationToken cancellationToken = default) =>
        await context.MailQueues.Where(predicate).ToListAsync(cancellationToken);
}
