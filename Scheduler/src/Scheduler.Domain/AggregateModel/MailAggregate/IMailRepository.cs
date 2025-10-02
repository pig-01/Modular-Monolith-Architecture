using Scheduler.Domain.Enums;

namespace Scheduler.Domain.AggregateModel.MailAggregate;

public interface IMailRepository : IRepository<MailQueue>
{
    Task<int> UpdateMailQueueStatusAsync(long mailQueueId, MailQueueStatus status, CancellationToken cancellationToken);
}
