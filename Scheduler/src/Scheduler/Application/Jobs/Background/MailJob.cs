using Scheduler.Application.Commands.Mails;
using Scheduler.Domain.SeedWork;
using MediatR;
using Quartz;

namespace Scheduler.Application.Jobs.Background;

/// <summary>
/// Mail job for processing and sending emails which is pending status.
/// </summary>
/// <remarks>
/// This job reads mail queue items from the database and sends them using the mail service.
/// It is designed to run periodically to ensure timely email delivery.
/// </remarks>
/// <typeparam name="MailJob">Mail job for processing and sending emails which is pending status.</typeparam>
public class MailJob(ILogger<MailJob> logger, IMediator mediator) : BaseJob(logger)
{
    public static readonly JobKey JobKey = new("MailJob");

    /// <summary>
    /// Executes the mail job
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override async Task Run(IJobExecutionContext context) =>
        _ = await mediator.Send(new SendDatabaseMailCommand(), context.CancellationToken);
}