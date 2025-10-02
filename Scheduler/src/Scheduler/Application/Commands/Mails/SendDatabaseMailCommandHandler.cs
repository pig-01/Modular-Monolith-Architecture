using System.Net.Mail;
using Base.Domain.Models.Mail;
using Scheduler.Application.Queries.Mails;
using Scheduler.Domain.AggregateModel.MailAggregate;
using Scheduler.Domain.Enums;
using MediatR;
using MailBase = Base.Infrastructure.Interface.Mail;

namespace Scheduler.Application.Commands.Mails;

/// <summary>
/// Handler for send database mail command.
/// </summary>
/// <typeparam name="SendDatabaseMailCommandHandler"></typeparam>
public class SendDatabaseMailCommandHandler(ILogger<SendDatabaseMailCommandHandler> logger,
    IMailQuery mailQuery,
    MailBase.IMailService mailService,
    IMailRepository mailRepository) : IRequestHandler<SendDatabaseMailCommand, bool>
{
    public async Task<bool> Handle(SendDatabaseMailCommand request, CancellationToken cancellationToken)
    {
        // Read mail queue items and process them
        IEnumerable<MailQueue> mailItems = await mailQuery.GetPendingMailItemsAsync(cancellationToken);

        // Create a list of tasks to send emails
        IEnumerable<Task<(long, bool)>> tasks = mailItems.Select(SendMailAsync);
        IEnumerable<(long, bool)> results = await Task.WhenAll(tasks);

        // Update mail queue status based on send results
        foreach ((long mailItemId, bool isSuccess) in results)
        {
            MailQueueStatus mailQueueStatus = isSuccess ? MailQueueStatus.Sent : MailQueueStatus.Failed;
            logger.LogInformation("Mail job for item {MailItemId} has been {MailQueueStatus}", mailItemId, mailQueueStatus.Id);
            _ = await mailRepository.UpdateMailQueueStatusAsync(mailItemId, mailQueueStatus, CancellationToken.None);
        }

        return true;
    }

    /// <summary>
    /// Send mail asynchronously
    /// </summary>
    /// <param name="mailItem"></param>
    /// <returns></returns>
    private async Task<(long, bool)> SendMailAsync(MailQueue mailItem)
    {
        try
        {
            // Create mail information
            MailInfomation mailInfomation = new()
            {
                ReceiverList = [.. mailItem.Recipient.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(email => new MailAddress(email))],
                Subject = mailItem.Subject,
                Body = mailItem.Body,
                IsBodyHtml = mailItem.IsBodyHtml
            };

            // Add CC recipients
            foreach (string email in mailItem.Cc?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>())
            {
                mailInfomation.CarbonCopyList.Add(new MailAddress(email));
            }

            // Add BCC recipients
            foreach (string email in mailItem.Bcc?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>())
            {
                mailInfomation.BlindCarbonCopyList.Add(new MailAddress(email));
            }

            // Implement the logic to send the email
            await mailService.SendAsync(mailInfomation);
        }
        catch (SmtpException smtpException)
        {
            logger.LogError(smtpException, "SMTP error occurred while sending email");
            return (mailItem.Id, false);
        }

        return (mailItem.Id, true);
    }
}
