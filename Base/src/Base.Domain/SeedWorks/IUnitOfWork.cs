using Microsoft.EntityFrameworkCore.Storage;

namespace Base.Domain.SeedWorks;

public interface IUnitOfWork
{
    IDbContextTransaction? Transaction { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task<TResponse> ExecuteAsync<TService, TResponse>(Func<Task<TResponse>> operation, string functionName, CancellationToken cancellationToken = default);

    Task<TResponse> ExecuteAsync<TService, TResponse>(Func<Task<TResponse>> operation, string functionName, Func<Exception, Task>? error, CancellationToken cancellationToken = default);

    Task ExecuteAsync<TService>(Func<Task> operation, string functionName, CancellationToken cancellationToken = default);

    Task ExecuteAsync<TService>(Func<Task> operation, string functionName, Func<Exception, Task>? error, CancellationToken cancellationToken = default);
}
