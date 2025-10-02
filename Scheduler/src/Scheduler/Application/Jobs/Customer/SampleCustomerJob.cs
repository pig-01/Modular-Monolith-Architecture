using Quartz;

namespace Scheduler.Application.Jobs.Customer;

public class SampleCustomerJob(ILogger<SampleCustomerJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Delay(1000, context.CancellationToken);
    }
}
