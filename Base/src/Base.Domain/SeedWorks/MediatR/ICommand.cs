using MediatR;

namespace Base.Domain.SeedWorks.MediatR;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
