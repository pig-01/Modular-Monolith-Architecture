using MediatR;

namespace Base.Domain.SeedWorks.MediatR;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{ }
