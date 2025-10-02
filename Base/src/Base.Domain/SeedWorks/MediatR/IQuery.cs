using MediatR;

namespace Base.Domain.SeedWorks.MediatR;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
