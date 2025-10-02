using Main.Domain.SeedWork;
using Main.Infrastructure.Demo.Context;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Main.Infrastructure;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DemoContext context)
    {
        IEnumerable<EntityEntry<Entity>> domainEntities = context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents is not null && x.Entity.DomainEvents.Count is not 0);

        List<INotification> domainEvents = [.. domainEntities.SelectMany(x => x.Entity.DomainEvents)];

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (INotification domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
