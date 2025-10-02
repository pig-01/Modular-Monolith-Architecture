using Microsoft.Extensions.Logging;
using Quartz;

namespace Scheduler.Domain.SeedWork;

public abstract class BaseJob(ILogger logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        DateTime startTime = DateTime.UtcNow;
        logger.LogInformation("Job started at: {StartTime:yyyy-MM-dd HH:mm:ss}", startTime);

        try
        {
            await Run(context); // 呼叫衍生類別的實際工作邏輯
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the job.");
        }
        finally
        {
            DateTime endTime = DateTime.UtcNow;
            logger.LogInformation("Job ended at: {EndTime:yyyy-MM-dd HH:mm:ss}", endTime);
        }
    }

    // 讓子類別覆寫這個方法，寫自己的工作內容
    protected abstract Task Run(IJobExecutionContext context);
}