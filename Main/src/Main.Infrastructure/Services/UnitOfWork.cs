using Dapper;
using Base.Domain.Exceptions;
using Base.Domain.SeedWorks;
using Main.Infrastructure.Demo.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Main.Infrastructure.Services;

public class UnitOfWork(ILogger<UnitOfWork> logger, DemoContext context) : IUnitOfWork
{
    public IDbContextTransaction? Transaction { get; private set; }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Transaction = await context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null) throw new HandleException("Transaction is null");
        await Transaction.CommitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null) throw new HandleException("Transaction is null");
        await Transaction.RollbackAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    public async Task<TResponse> ExecuteAsync<TService, TResponse>(Func<Task<TResponse>> operation, string functionName, CancellationToken cancellationToken = default) => await ExecuteAsync<TService, TResponse>(operation, functionName, default, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    public async Task<TResponse> ExecuteAsync<TService, TResponse>(Func<Task<TResponse>> operation, string functionName, Func<Exception, Task>? error, CancellationToken cancellationToken = default)
    {
        TResponse? response = default;
        string typeName = typeof(TService).Name;

        try
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(operation, async delegate (Func<Task<TResponse>> operationScoped, CancellationToken cancellationToken)
            {
                Guid transactionId;

                await using IDbContextTransaction transaction = await context.BeginTransactionAsync();
                using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    logger.LogInformation("Begin transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);
                    response = await operationScoped().ConfigureAwait(continueOnCapturedContext: false);
                    logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                    await context.CommitTransactionAsync(transaction, cancellationToken);

                    transactionId = transaction.TransactionId;
                }
            }, cancellationToken);
        }
        catch (WarningException ex)
        {
            logger.LogWarning(ex, "Warn Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
        }
        catch (HandleException ex)
        {
            logger.LogError(ex, "Error Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unknown Error Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
            throw;
        }

        return response ?? throw new NotFoundException("Response is default value");
    }

    public async Task ExecuteAsync<TService>(Func<Task> operation, string functionName, CancellationToken cancellationToken = default) => await ExecuteAsync<TService>(operation, functionName, null, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

    public async Task ExecuteAsync<TService>(Func<Task> operation, string functionName, Func<Exception, Task>? error, CancellationToken cancellationToken = default)
    {
        string typeName = typeof(TService).Name;

        try
        {
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(operation, async delegate (Func<Task> operationScoped, CancellationToken cancellationToken)
            {
                Guid transactionId;

                await using IDbContextTransaction? transaction = await context.BeginTransactionAsync();
                using (logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    logger.LogInformation("Begin transaction {TransactionId} for {CommandName} {FunctionName}", transaction.TransactionId, typeName, functionName);
                    await operationScoped().ConfigureAwait(continueOnCapturedContext: false);
                    logger.LogInformation("Commit transaction {TransactionId} for {CommandName} {FunctionName}", transaction.TransactionId, typeName, functionName);

                    await context.CommitTransactionAsync(transaction, cancellationToken);

                    transactionId = transaction.TransactionId;
                }
            }, cancellationToken);
        }
        catch (WarningException ex)
        {
            logger.LogWarning(ex, "Warn Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
        }
        catch (HandleException ex)
        {
            logger.LogError(ex, "Error Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unknown Error Handling transaction for {CommandName} {FunctionName} ({@Command})", typeName, functionName, ex);
            await DoCustomerError(error, ex);
            throw;
        }
    }

    private static async Task DoCustomerError(Func<Exception, Task>? error, Exception? exception = default)
    {
        if (error is null) return;

        if (exception is null)
        {
            await error(new Exception("Unknown Error")).ConfigureAwait(continueOnCapturedContext: false);
        }
        else
        {
            await error(exception).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
