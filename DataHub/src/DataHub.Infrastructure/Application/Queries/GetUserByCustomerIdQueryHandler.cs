using Base.Domain.Exceptions;
using Base.Domain.SeedWorks.MediatR;
using DataHub.Domain.AggregatesModel.CustomerAggregate;
using DataHub.Domain.AggregatesModel.UserAggregate;
using DataHub.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataHub.Infrastructure.Application.Queries;

public class GetUserByCustomerIdQueryHandler(
    ILogger<GetUserByCustomerIdQueryHandler> logger,
    DemoContext context) : IQueryHandler<GetUserByCustomerIdQuery, Scuser>
{
    public async Task<Scuser> Handle(GetUserByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        CustomerScuserRelation? customerScuserRelation =
            await context
                .CustomerScuserRelations
                .FirstOrDefaultAsync(x => x.CustomerId.Replace("-", "") == request.CustomerId, cancellationToken);

        if (customerScuserRelation == null)
        {
            logger.LogError("CustomerScuserRelation not found for customerId: {customerId}", request.CustomerId);
            throw new HandleException("CustomerScuserRelation not found");
        }

        Scuser? user = await context
            .Scusers
            .FirstOrDefaultAsync(x => x.UserId == customerScuserRelation.UserId, cancellationToken);

        if (user == null)
        {
            logger.LogError("User not found for userId: {userId}", customerScuserRelation.UserId);
            throw new HandleException("CustomerScuserRelation not found");
        }

        return user;
    }
}
