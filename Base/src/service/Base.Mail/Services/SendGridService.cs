using System.Net.Mail;
using Base.Domain.Models.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Base.Mail.Services;

public class SendGridService(MailServiceParameter mailServiceParameter)
{
    public async Task SendMailAsync(MailMessage mailMessage, CancellationToken cancellationToken = default)
    {
        SendGridClient sendGridClient = new(mailServiceParameter.Password);
        await sendGridClient.SendEmailAsync(ChangeMailMessage(mailMessage), cancellationToken);
    }

    private static SendGridMessage ChangeMailMessage(MailMessage mailMessage)
    {
        SendGridMessage sendGridMessage = new()
        {
            From = new EmailAddress(mailMessage.From.Address, mailMessage.From.DisplayName),
            Subject = mailMessage.Subject,
            HtmlContent = mailMessage.IsBodyHtml ? mailMessage.Body : null,
            PlainTextContent = mailMessage.IsBodyHtml ? null : mailMessage.Body
        };
        sendGridMessage.AddTos([.. mailMessage.To.Select(x => new EmailAddress(x.Address, x.DisplayName))]);
        if (mailMessage.CC.Any()) sendGridMessage.AddCcs([.. mailMessage.CC.Select(x => new EmailAddress(x.Address, x.DisplayName))]);
        if (mailMessage.Bcc.Any()) sendGridMessage.AddBccs([.. mailMessage.Bcc.Select(x => new EmailAddress(x.Address, x.DisplayName))]);

        return sendGridMessage;
    }
}
