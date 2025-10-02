using System.Linq.Expressions;

namespace Scheduler.Domain.SeedWork;

public interface IQuery<T>
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
