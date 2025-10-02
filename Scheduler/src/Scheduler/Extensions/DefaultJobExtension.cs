using Scheduler.Application.Jobs.Background;
using Scheduler.Application.Jobs.Plans;
using Quartz;

namespace Scheduler.Extensions;

public static class DefaultJobExtension
{
    public static void UseDefaultBackgroundJobStore(this IServiceCollectionQuartzConfigurator configurator)
    {
        configurator.AddJob<MailJob>(opts => opts.WithIdentity(MailJob.JobKey).StoreDurably())
            .AddTrigger(opts => opts
                .ForJob(MailJob.JobKey)
                .WithIdentity($"{MailJob.JobKey.Name}.trigger")
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(30)) // 30 seconds later
                .WithCronSchedule("0 0 * * * ?") // every hour
                );

        configurator.AddJob<ExpiredFormCheckJob>(opts => opts.WithIdentity(ExpiredFormCheckJob.JobKey).StoreDurably())
            .AddTrigger(opts => opts
                .ForJob(ExpiredFormCheckJob.JobKey)
                .WithIdentity($"{ExpiredFormCheckJob.JobKey.Name}.trigger")
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(30)) // 30 seconds later
                .WithCronSchedule("0 0/1 * * * ?")); // every minute
    }

}
