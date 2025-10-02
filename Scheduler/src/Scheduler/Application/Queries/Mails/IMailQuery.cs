using Scheduler.Domain.AggregateModel.MailAggregate;
using Scheduler.Domain.SeedWork;

namespace Scheduler.Application.Queries.Mails;

public interface IMailQuery : IQuery<MailQueue>, Base.Infrastructure.Interface.Mail.IMailRepository
{
    /// <summary>
    /// 取得寄件者資訊
    /// </summary>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<MailSender> GetMailSender(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得待處理的發信佇列
    /// </summary>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns>發信佇列清單</returns>
    Task<IEnumerable<MailQueue>> GetPendingMailItemsAsync(CancellationToken cancellationToken = default);
}
