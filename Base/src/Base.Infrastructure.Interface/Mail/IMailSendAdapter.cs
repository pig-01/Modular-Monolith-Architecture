using System.Net.Mail;
using Base.Domain.Models.Mail;

namespace Base.Infrastructure.Interface.Mail;

public interface IMailSendAdapter
{
    Task SendMail(MailInfomation mailInfomation, MailServiceParameter mailServiceParameter, CancellationToken cancellationToken = default);

    Task SendMail(MailMessage mailMessage, MailServiceParameter mailServiceParameter, CancellationToken cancellationToken = default);
}
