using System.Globalization;
using Scheduler.Domain.AggregateModel.MailAggregate;
using Scheduler.Domain.Enums;
using Scheduler.Infrastructure.Context;

namespace Scheduler.Infrastructure.Repositories.Mails;

public class MailRepository(DemoContext context) : IMailRepository
{
    public async Task<MailQueue> AddAsync(MailQueue entity, CancellationToken cancellationToken = default)
    {
        context.MailQueues.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(MailQueue entity, CancellationToken cancellationToken = default)
    {
        context.MailQueues.Remove(entity);
        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MailQueue entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).CurrentValues.SetValues(entity);
        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateMailQueueStatusAsync(long mailQueueId, MailQueueStatus status, CancellationToken cancellationToken)
    {
        MailQueue? mailQueue = await context.MailQueues.FirstOrDefaultAsync(x => x.Id == mailQueueId, cancellationToken);
        if (mailQueue is not null)
        {
            mailQueue.Status = status.Id.ToString(CultureInfo.InvariantCulture);
            return await context.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }
}
