using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Commands;
using Order.Application.Pipeline;
using Order.Application.Validation;
using Order.Infrastructure;

namespace Order.IntegrationTest;

public class PlaceOrderCommandHandlerTests
{
    [Fact]
    public async Task Places_order_and_returns_dto()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("order-int-tests"));
        services.AddMediatR(typeof(PlaceOrderCommand).Assembly);
        services.AddValidatorsFromAssembly(typeof(PlaceOrderCommand).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new PlaceOrderCommand(Guid.NewGuid(), new[]
        {
            new PlaceOrderItem(Guid.NewGuid(), 1, 9.99m)
        }));

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Single(result.Items);
        Assert.Equal(9.99m, result.Total);
    }
}
