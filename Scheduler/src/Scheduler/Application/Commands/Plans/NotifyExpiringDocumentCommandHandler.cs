using System.Globalization;
using Base.Domain.Models.Mail;
using Scheduler.Application.Commands.Mails;
using Scheduler.Application.Models.Mails;
using Scheduler.Application.Queries.Plans;
using Scheduler.Domain.AggregateModel.PlanAggregate;
using MediatR;
using Razor.Templating.Core;
using MailBase = Base.Infrastructure.Interface.Mail;

namespace Scheduler.Application.Commands.Plans;

/// <summary>
/// 過期表單通知命令處理器
/// </summary>
public class NotifyExpiringDocumentCommandHandler(
    ILogger<NotifyExpiringDocumentCommandHandler> logger,
    IPlanDocumentQuery planDocumentQuery,
    IMediator mediator,
    MailBase.IMailService mailService)
    : IRequestHandler<NotifyExpiringDocumentCommand, NotifyExpiringDocumentResult>
{
    public async Task<NotifyExpiringDocumentResult> Handle(NotifyExpiringDocumentCommand request, CancellationToken cancellationToken)
    {
        NotifyExpiringDocumentResult result = new()
        {
            IsSuccess = true
        };

        try
        {
            if (!request.IsWarningNotification) throw new NotImplementedException("目前僅支援預警通知功能");

            logger.LogInformation("Starting to process expiring documents - DaysUntilExpiration: {DaysUntilExpiration}, IsWarningNotification: {IsWarningNotification}",
                request.DaysUntilExpiration, request.IsWarningNotification);

            // 1. 查詢過期或即將過期的表單
            IEnumerable<PlanDocument> expiringDocuments = await planDocumentQuery.GetExpiringDocumentsAsync(
                request.DaysUntilExpiration, cancellationToken);

            if (!expiringDocuments.Any())
            {
                logger.LogInformation("No expiring documents found");
                return result;
            }

            logger.LogInformation("Found {Count} expiring documents", expiringDocuments.Count());

            // 2. 按負責人分組，再按計畫分類
            List<IGrouping<string, PlanDocument>> responsibleGroups = [.. expiringDocuments
                .Where(doc => !string.IsNullOrWhiteSpace(doc.Responsible) && IsValidEmail(doc.Responsible))
                .GroupBy(doc => doc.Responsible!)];

            // 3. 為每個負責人發送一封彙總郵件
            foreach (IGrouping<string, PlanDocument>? responsibleGroup in responsibleGroups)
            {
                try
                {
                    await ProcessResponsibleDocuments(responsibleGroup.Key, responsibleGroup, request, result, cancellationToken);
                    result.ProcessedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while processing documents for responsible {Responsible}", responsibleGroup.Key);
                }
            }

            logger.LogInformation("Check for expired forms completed - Processed: {ProcessedCount}, Notifications Sent: {NotificationsSent}",
                result.ProcessedCount, result.NotificationsSent);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing expired form notifications");
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 處理單個負責人的所有過期表單
    /// </summary>
    private async Task ProcessResponsibleDocuments(
        string responsible,
        IEnumerable<PlanDocument> documents,
        NotifyExpiringDocumentCommand request,
        NotifyExpiringDocumentResult result,
        CancellationToken cancellationToken = default)
    {
        // 按計畫分組表單
        List<PlanDocumentGroup> planGroups = [.. documents
            .GroupBy(doc => new
            {
                 doc.PlanDetail.Plan.PlanId,
                 doc.PlanDetail.Plan.PlanName,
                 doc.PlanDetail.Plan.TenantId
            })
            .Select(group => new PlanDocumentGroup
            {
                PlanId = group.Key.PlanId,
                PlanName = group.Key.PlanName,
                TenantId = group.Key.TenantId,
                Documents = [.. group.Select(doc => new DocumentSummary
                {
                    PlanDocumentId = doc.PlanDocumentId,
                    PlanDetailName = doc.PlanDetail.PlanDetailName,
                    Code = doc.PlanDetail.RowNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                    EndDate = doc.EndDate,
                    FormStatus = doc.FormStatus,
                    DaysOverdue = (int)(DateTime.Now.Date - doc.EndDate.Date).TotalDays,
                    Year = doc.Year,
                    Quarter = doc.Quarter,
                    Month = doc.Month
                })]
            })];

        // 建立通知模型
        ResponsibleNotificationModel notificationModel = new()
        {
            Responsible = responsible,
            TotalDocuments = documents.Count(),
            PlanGroups = planGroups,
            NotificationType = request.IsWarningNotification ? "預警通知" : "過期通知",
            DaysUntilExpiration = request.DaysUntilExpiration
        };

        // 更新結果摘要
        foreach (PlanDocument doc in documents)
        {
            ExpiredDocumentSummary summary = new()
            {
                PlanDocumentId = doc.PlanDocumentId,
                Responsible = doc.Responsible,
                EndDate = doc.EndDate,
                FormStatus = doc.FormStatus,
                DaysOverdue = (int)(DateTime.Now.Date - doc.EndDate.Date).TotalDays
            };
            result.DocumentSummaries.Add(summary);
        }

        // 渲染郵件內容
        string body = await RazorTemplateEngine.RenderAsync("~/Templates/ResponsibleNotification.cshtml", notificationModel);

        // 取得郵件範本資訊
        string functionCode = "Plan";
        string mailType = request.IsWarningNotification ? "Expiring" : "Expired";

        MailInfomation mailInfomation = await mailService.GetMailTemplate(functionCode, mailType, cancellationToken);

        // 設定郵件主旨
        string subject = request.IsWarningNotification
            ? $"Demo表單即將過期提醒 - {notificationModel.TotalDocuments}份表單"
            : $"Demo表單過期通知 - {notificationModel.TotalDocuments}份表單";

        bool isSuccess = await mediator.Send(new InsertDatabaseMailCommand(
            [responsible],
            subject,
            body,
            true), cancellationToken);

        if (isSuccess) result.NotificationsSent++;

        logger.LogInformation("Notification for responsible {Responsible} queued - {DocumentCount} documents across {PlanCount} plans",
            responsible, notificationModel.TotalDocuments, notificationModel.PlanGroups.Count);
    }

    /// <summary>
    /// 簡單的 Email 格式驗證
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            System.Net.Mail.MailAddress addr = new(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}