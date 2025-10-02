using Base.Domain.Exceptions;
using Main.Infrastructure.Demo.Context;
using Main.WebApi.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Main.WebApi.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(DemoContext context,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{

    public class NetZeroApiResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;

        public static NetZeroApiResult Success(string message = "操作成功")
        {
            return new NetZeroApiResult { IsSuccess = true, Message = message };
        }

        public static NetZeroApiResult Error(string errorCode, string message)
        {
            return new NetZeroApiResult { IsSuccess = false, ErrorCode = errorCode, Message = message };
        }
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse? response = default;
        string typeName = request.GetGenericTypeName();

        try
        {
            if (context.HasActiveTransaction)
            {
                return await next();
            }

            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using IDbContextTransaction transaction = await context.BeginTransactionAsync();
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
        }
        catch (WarningException wex)
        {
            logger.LogWarning(wex, "Warning Handling transaction for {CommandName} ({@Command})", typeName, request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);

            throw;
        }

        return response;
    }
}
