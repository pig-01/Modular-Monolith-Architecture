using System.Net.Mail;
using Base.Domain.Exceptions;
using Base.Domain.Models.Mail;
using Base.Infrastructure.Interface.Mail;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Models.Mail;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.Users;
using Razor.Templating.Core;

namespace Main.WebApi.Application.Commands.Plans;

public class NotifyPlanDocumentCommandHandler(
    ILogger<NotifyPlanDocumentCommandHandler> logger,
    IUserQuery userQuery,
    IPlanQuery planQuery,
    IPlanDetailQuery planDetailQuery,
    IHttpContextAccessor httpContextAccessor,
    IMailService mailService) : IRequestHandler<NotifyPlanDocumentCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(NotifyPlanDocumentCommand request, CancellationToken cancellationToken)
    {
        // 檢查計畫是否存在
        Plan plan = await planQuery.GetByIdAsync(request.PlanId, cancellationToken) ?? throw new NotFoundException("Plan is not found");

        IEnumerable<PlanDetail> planDetails = await planDetailQuery.ListByPlanIdAsync(plan.PlanId, cancellationToken);

        var filteredPlanDetails = planDetails
            .Where(p => request.PlanDetailIdList.Contains(p.PlanDetailId))
            .Where(p => !string.IsNullOrEmpty(p.ResponsibleList)).ToArray();

        if (filteredPlanDetails.Count() == 0)
        {
            return Unit.Value;
        }

        foreach (var userId in filteredPlanDetails[0].ResponsibleList!.Split(","))
        {
            // 檢查負責人是否存在
            Scuser responsible = await userQuery.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("Responsible person is not found");


            logger.LogInformation($"負責人: {responsible}, 負責的計畫明細數量: {filteredPlanDetails.Length}");
            // 在這裡可以對每個負責人的計畫明細進行處理

            try
            {
                string mailSubject = "提醒您填寫 專案盤查計畫：";

                // 準備郵件內容
                // 使用 Razor 模板引擎渲染郵件中動態內容
                // 注意：這裡的 RazorTemplateEngine.RenderAsync 方法需要根據實際情況進行實現或調整
                DocumentAssignNotificationModel notificationModel = new(
                    "提醒您填寫 專案盤查計畫：",
                    plan.PlanName,
                    null,
                    null,
                    [.. filteredPlanDetails.Select(p => ConvertToAssignItem(p, plan))]);

                string renderedTable = await RazorTemplateEngine.RenderAsync("~/Templates/DocumentNotificationTemplate.cshtml", notificationModel);

                // 取得郵件範本資訊
                // 注意：這裡的 GetMailTemplate 方法需要根據實際情況進行實現或調整
                // 這裡的 functionCode 和 mailType 需要根據實際情況進行設置
                string functionCode = "plan";
                string mailType = "Assign";

                MailInfomation mailInfomation = await mailService.GetMailTemplate(functionCode, mailType, cancellationToken);
                mailInfomation.ReceiverList.Add(new MailAddress(responsible.UserId, responsible.UserName));
                mailInfomation.Subject = "[demo Demo] 提醒您填寫 專案盤查計畫「" + plan.PlanName + "」";
                mailInfomation.Body = mailInfomation.Body?.Replace("{Body}", renderedTable);

                await mailService.SendAsync(mailInfomation, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to handle PlanDocumentAssignedDomainEvent: {Message}", ex.Message);
                throw new WarningException("Failed to handle PlanDocumentAssignedDomainEvent", ex);
            }
        }

        return Unit.Value;
    }

    private AssignItem ConvertToAssignItem(PlanDetail planDetail, Plan plan)
    {

        int[] cycles = planDetail.CycleType switch
        {
            "month" => Enumerable.Range(1, 12).Where(m => !planDetail.PlanDocuments.Any(pd => pd.Month == m)).ToArray(),
            "quarter" => Enumerable.Range(1, 4).Where(q => !planDetail.PlanDocuments.Any(pd => pd.Quarter == q)).ToArray(),
            "year" => [int.Parse(plan.Year)],
            _ => []
        };

        string requestUrl = httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString(); // 使用者從哪個頁面過來的

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
