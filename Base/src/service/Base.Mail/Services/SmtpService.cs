using System.Net.Mail;
using Base.Domain.Models.Mail;

namespace Base.Mail.Services;

public class SmtpService(MailServiceParameter mailServiceParameter)
{
    public async Task SendMailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        using SmtpClient smtpClient = new(mailServiceParameter.Domain)
        {
            Credentials = new System.Net.NetworkCredential(mailServiceParameter.Account, mailServiceParameter.Password),
            EnableSsl = mailServiceParameter.EnableSSL,
            Port = 25
        };
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
