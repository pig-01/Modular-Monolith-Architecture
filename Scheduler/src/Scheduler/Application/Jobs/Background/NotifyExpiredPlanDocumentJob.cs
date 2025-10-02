using System.Globalization;
using Base.Infrastructure.Interface.Mail;
using Scheduler.Application.Queries.Plans;
using Scheduler.Domain.AggregateModel.MailAggregate;
using Scheduler.Domain.AggregateModel.PlanAggregate;
using Scheduler.Domain.Enums;
using Scheduler.Domain.SeedWork;
using Quartz;
using MailBase = Base.Domain.Models.Mail;

namespace Scheduler.Application.Jobs.Background;

/// <summary>
/// Job that sends notifications for expired plan documents.
/// </summary>
public class NotifyExpiredPlanDocumentJob(
    ILogger<NotifyExpiredPlanDocumentJob> logger,
    IMailService mailService,
    IPlanDocumentQuery planDocumentQuery) : BaseJob(logger)
{
    protected override async Task Run(IJobExecutionContext context)
    {
        // TODO step1 查詢是否有過期的 plan document
        IEnumerable<PlanDocument> expiredDocuments = await FindExpiredPlanDocumentsAsync();

        // TODO step2 發送通知郵件
        IEnumerable<Task> tasks = expiredDocuments.Select(SendExpiredDocumentNotificationAsync);
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Sends a notification email for an expired plan document.
    /// </summary>
    /// <param name="document">plan document</param>
    /// <returns></returns>
    private async Task SendExpiredDocumentNotificationAsync(PlanDocument document)
    {
        MailBase.MailInfomation mailInfomation = await mailService.GetMailTemplate("ExpiredPlanDocument", "Notification");

        // Create mail infomation into database mail queue
        MailQueue mailQueue = new(
            [document.Responsible ?? document.CreatedUser],
            [],
            mailInfomation.Subject,
            mailInfomation.Body,
            TenantEnum.SuperTenant.Name,
            "System",
            isHtml: true);
    }

    /// <summary>
    /// Finds plan documents that have expired.
    /// </summary>
    /// <returns></returns>
    private async Task<IEnumerable<PlanDocument>> FindExpiredPlanDocumentsAsync()
    {
        IEnumerable<PlanDocument> planDocuments = await planDocumentQuery.ListAsync(
            x => x.FormStatus == DocumentStatus.UnWritten.Id.ToString(CultureInfo.CurrentCulture));

        return planDocuments;
    }
}
