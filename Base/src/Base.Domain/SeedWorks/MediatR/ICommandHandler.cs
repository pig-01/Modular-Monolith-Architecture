using MediatR;

namespace Base.Domain.SeedWorks.MediatR;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{ }
