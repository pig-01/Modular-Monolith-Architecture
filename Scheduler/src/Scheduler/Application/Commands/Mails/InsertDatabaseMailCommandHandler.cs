using Scheduler.Domain.AggregateModel.MailAggregate;
using MediatR;

namespace Scheduler.Application.Commands.Mails;

/// <summary>
/// 插入資料庫郵件命令處理器
/// </summary>
/// <typeparam name="InsertDatabaseMailCommandHandler"></typeparam>
public class InsertDatabaseMailCommandHandler(
    ILogger<InsertDatabaseMailCommandHandler> logger,
    IMailRepository mailRepository
) : IRequestHandler<InsertDatabaseMailCommand, bool>
{
    public async Task<bool> Handle(InsertDatabaseMailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Inserting mail into database for recipients: {Recipients}", string.Join(", ", request.Recipients));

            string systemName = Environment.GetEnvironmentVariable("SYSTEM_NAME") ?? "專案System";
            MailQueue mailQueue = new(request.Recipients, request.Subject, request.Body, systemName, request.IsBodyHtml);

            _ = await mailRepository.AddAsync(mailQueue, cancellationToken);
            logger.LogInformation("Mail inserted into database with ID: {MailQueueId}", mailQueue.Id);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inserting mail into database");
            return false;
        }
    }
}
