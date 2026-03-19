using MediatR;

namespace User.IntegrationEvent.Events;

public record UserCreatedIntegrationEvent(Guid UserId, string Name, string Email) : INotification;
