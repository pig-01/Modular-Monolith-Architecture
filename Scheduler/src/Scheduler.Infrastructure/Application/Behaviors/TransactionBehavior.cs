using Base.Domain.Exceptions;
using Scheduler.Infrastructure.Context;
using Scheduler.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Scheduler.Infrastructure.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(DemoContext context,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse? response = default;
        string typeName = request.GetGenericTypeName();

        try
        {
            if (context.HasActiveTransaction)
            {
                return await next(cancellationToken);
            }

            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using IDbContextTransaction? transaction = await context.BeginTransactionAsync();
                using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                    response = await next();

                    logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                    await context.CommitTransactionAsync(transaction);

                    transactionId = transaction.TransactionId;
                }

                // await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);

            });

            return response;
        }
        catch (WarningException ex)
        {
            logger.LogWarning(ex, "Warning Handling transaction for {CommandName} ({@Command})", typeName, request);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);
            throw;
        }
    }
}
