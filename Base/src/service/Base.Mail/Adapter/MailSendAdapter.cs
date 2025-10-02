using System.Net.Mail;
using Base.Domain.Models.Mail;
using Base.Infrastructure.Interface.Mail;
using Base.Mail.Services;
using Microsoft.Extensions.Logging;

namespace Base.Mail.Adapter;

public class MailSendAdapter(ILogger<MailSendAdapter> logger) : IMailSendAdapter
{
    /// <summary>
    /// 發送郵件 with MailInfomation
    /// </summary>
    /// <param name="mailInfomation">郵件訊息</param>
    /// <param name="mailServiceParameter">郵件發信機參數</param>
    /// <param name="cancellationToken"></param>
    public async Task SendMail(MailInfomation mailInfomation, MailServiceParameter mailServiceParameter, CancellationToken cancellationToken = default)
    {
        MailMessage mailMessage = new()
        {
            From = mailInfomation.Sender ?? new MailAddress(mailServiceParameter.GetMailSender(), mailServiceParameter.Account),
            Subject = mailInfomation.Subject,
            Body = mailInfomation.Body,
            IsBodyHtml = mailInfomation.IsBodyHtml,
        };

        // 添加收件者、抄送者和密件抄送者
        mailInfomation.ReceiverList.ForEach(mailMessage.To.Add);
        mailInfomation.CarbonCopyList.ForEach(mailMessage.CC.Add);
        mailInfomation.BlindCarbonCopyList.ForEach(mailMessage.Bcc.Add);

        await SendMail(mailMessage, mailServiceParameter, cancellationToken);
    }

    /// <summary>
    /// 發送郵件 with MailMessage
    /// </summary>
    /// <param name="mailMessage">郵件內容</param>
    /// <param name="mailServiceParameter">郵件發信機參數</param>
    /// <param name="cancellationToken"></param>
    public async Task SendMail(MailMessage mailMessage, MailServiceParameter mailServiceParameter, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Start to send mail with {ServiceType}", mailServiceParameter.ServiceType);
        switch (mailServiceParameter.ServiceType)
        {
            case "SmtpSendAdapter":
                SmtpService smtpService = new(mailServiceParameter);
                await smtpService.SendMailAsync(mailMessage, cancellationToken);
                break;
            case "SendGridAdapter":
                SendGridService sendGridService = new(mailServiceParameter);
                await sendGridService.SendMailAsync(mailMessage, cancellationToken);
                break;
            default: throw new NotImplementedException($"Mail Service Type '{mailServiceParameter.ServiceType}' is not implemented.");
        }
    }
}
