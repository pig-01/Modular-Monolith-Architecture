using Base.Infrastructure.Interface.Mail;
using DataHub.Domain.Events;
using MediatR;

namespace DataHub.Cloud.Application.DomainEventHandlers;

internal class SendProvisionMailDomainEventHandler(
    ILogger<SendProvisionMailDomainEventHandler> logger,
    IMailSendAdapter mailSendAdapter) : INotificationHandler<SendProvisionMailDomainEvent>
{
    public async Task Handle(SendProvisionMailDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("寄發供裝成功信件");
            //MailServiceParameter mailServiceParameter = mailSendAdapter.GetMailServiceParameter("demoHCM", "ProvisionCompleted", "zh-CHT");
            //MailInfomation mailInfomation = mailSendAdapter.GetMailTemplate("demoHCM", "ProvisionCompleted", "zh-CHT", "zh-CHT", "zh-CHT");
            //mailSendAdapter.SendMail(mailInfomation, mailServiceParameter, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "寄發供裝成功信件失敗");
            logger.LogInformation("寄發供裝成功信件");
        }
    }
}
