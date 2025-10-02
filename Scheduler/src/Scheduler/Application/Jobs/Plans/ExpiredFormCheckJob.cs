using Scheduler.Application.Commands.Plans;
using MediatR;
using Quartz;

namespace Scheduler.Application.Jobs.Plans;

/// <summary>
/// 過期表單檢查排程任務
/// </summary>
[DisallowConcurrentExecution]
public class ExpiredFormCheckJob(ILogger<ExpiredFormCheckJob> logger, IMediator mediator) : IJob
{
    public static readonly JobKey JobKey = new("ExpiredFormCheckJob");

    /// <summary>
    /// 執行過期表單檢查
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        string jobName = context.JobDetail.Key.Name;
        logger.LogInformation("Starting expired form check job: {JobName}", jobName);

        try
        {
            // 檢查已過期的表單（當天或已過期）
            NotifyExpiringDocumentCommand notifyExpiredCommand = new(0, false, "demo-Demo-scheduler");
            NotifyExpiringDocumentResult expiredResult = await mediator.Send(notifyExpiredCommand, context.CancellationToken);

            logger.LogInformation("Check for expired forms completed - Processed: {ProcessedCount}, Notifications Sent: {NotificationsSent}",
                expiredResult.ProcessedCount, expiredResult.NotificationsSent);

            // 檢查即將過期的表單（7天內到期）
            NotifyExpiringDocumentCommand notifyWarningCommand = new(7, true, "demo-Demo-scheduler");
            NotifyExpiringDocumentResult warningResult = await mediator.Send(notifyWarningCommand, context.CancellationToken);

            logger.LogInformation("Check for expiring forms completed - Processed: {ProcessedCount}, Notifications Sent: {NotificationsSent}",
                warningResult.ProcessedCount, warningResult.NotificationsSent);

            // 記錄總計結果
            int totalProcessed = expiredResult.ProcessedCount + warningResult.ProcessedCount;
            int totalNotifications = expiredResult.NotificationsSent + warningResult.NotificationsSent;

            logger.LogInformation("Check for expired forms completed - Total Processed: {TotalProcessed}, Total Notifications Sent: {TotalNotifications}",
                totalProcessed, totalNotifications);

            // 設定下次執行時間資訊到 JobDataMap
            context.JobDetail.JobDataMap.Put("LastExecutionTime", DateTime.Now);
            context.JobDetail.JobDataMap.Put("LastProcessedCount", totalProcessed);
            context.JobDetail.JobDataMap.Put("LastNotificationCount", totalNotifications);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing expired form check job: {JobName}", jobName);

            // 重新拋出例外以讓 Quartz 知道任務執行失敗
            throw;
        }
    }
}