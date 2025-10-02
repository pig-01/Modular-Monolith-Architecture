using System.Net.Mail;
using Base.Domain.Exceptions;
using Base.Domain.Models.Mail;
using Base.Infrastructure.Interface.Mail;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.Events.PlanAggregate;
using Main.WebApi.Application.Models.Mail;
using Razor.Templating.Core;

namespace Main.WebApi.Application.DomainEventHandlers.Plans;

/// <summary>
/// 在指標計劃文件指派後所執行的事件處理器
/// </summary>
public class PlanDocumentAssignedDomainEventHandler(
    ILogger<PlanDocumentAssignedDomainEventHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IMailService mailService) : INotificationHandler<PlanDocumentAssignedDomainEvent>
{
    public async Task Handle(PlanDocumentAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // 準備郵件內容
            // 使用 Razor 模板引擎渲染郵件中動態內容
            // 注意：這裡的 RazorTemplateEngine.RenderAsync 方法需要根據實際情況進行實現或調整
            DocumentAssignNotificationModel notificationModel = new(
                notification.Assign.UserName + " 透過 demo Demo指派您填寫 專案盤查計畫：",
                notification.PlanName,
                notification.Assign,
                notification.Responsible,
                [.. notification.PlanDetails.Select(ConvertToAssignItem)]);

            string renderedTable = await RazorTemplateEngine.RenderAsync("~/Templates/DocumentNotificationTemplate.cshtml", notificationModel);

            // 取得郵件範本資訊
            // 注意：這裡的 GetMailTemplate 方法需要根據實際情況進行實現或調整
            // 這裡的 functionCode 和 mailType 需要根據實際情況進行設置
            string functionCode = "plan";
            string mailType = "Assign";

            MailInfomation mailInfomation = await mailService.GetMailTemplate(functionCode, mailType, cancellationToken);
            mailInfomation.ReceiverList.Add(new MailAddress(notification.Responsible.UserId, notification.Assign.UserName));
            mailInfomation.Subject = mailInfomation.Subject?
                .Replace("{AssignUser}", notification.Assign.UserName)
                .Replace("{PlanName}", notification.PlanName);
            mailInfomation.Body = mailInfomation.Body?.Replace("{Body}", renderedTable);

            await mailService.SendAsync(mailInfomation, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to handle PlanDocumentAssignedDomainEvent: {Message}", ex.Message);
            throw new WarningException("Failed to handle PlanDocumentAssignedDomainEvent", ex);
        }
    }

    private AssignItem ConvertToAssignItem(PlanDetail planDetail)
    {
        int[] cycles = planDetail.CycleType switch
        {
            "month" => planDetail.PlanDocuments
                                  .Select(c => c.Month)
                                  .Where(m => m.HasValue)
                                  .Select(m => m.Value)
                                  .ToArray(),
            "quarter" => planDetail.PlanDocuments
                                  .Select(c => c.Quarter)
                                  .Where(q => q.HasValue)
                                  .Select(q => q.Value)
                                  .ToArray(),
            "year" => planDetail.PlanDocuments
                                  .Select(c => c.Year)
                                  .Where(y => y.HasValue)
                                  .Select(y => y.Value)
                                  .ToArray(),
            _ => Array.Empty<int>()
        };

        string requestUrl = httpContextAccessor.HttpContext.Request.Headers.Referer.ToString(); // 使用者從哪個頁面過來的

        return new AssignItem
        {
            PlanDetailId = planDetail.PlanDetailId,
            PlanDetailName = planDetail.PlanDetailName,
            RowNumber = planDetail.RowNumber,
            CycleType = planDetail.CycleType.ToString(),
            Cycles = cycles,
            CycleMonth = planDetail.CycleMonth,
            CycleDay = planDetail.CycleDay,
            CycleMonthLast = planDetail.CycleMonthLast ?? false,
            EndDate = planDetail.EndDate ?? throw new ArgumentNullException(nameof(planDetail.EndDate)),
            Url = new Uri($"{requestUrl}/#/plans/edit/{planDetail.Plan.PlanId}/details/{planDetail.PlanDetailId}")
        };
    }
}
