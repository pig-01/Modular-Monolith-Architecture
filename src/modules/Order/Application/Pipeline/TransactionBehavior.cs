using MediatR;
using System.Transactions;

namespace Order.Application.Pipeline;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted
        }, TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();
        scope.Complete();
        return response;
    }
}
