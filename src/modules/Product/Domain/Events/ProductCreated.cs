using MediatR;

namespace Product.Domain.Events;

public record ProductCreated(Guid ProductId, string Name, decimal Price) : INotification;
