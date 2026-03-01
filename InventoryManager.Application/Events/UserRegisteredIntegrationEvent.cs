using InventoryManager.Application.Abstractions.Messaging;

namespace InventoryManager.Application.Events;

public sealed class UserRegisteredIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public Guid UserId { get; init; }

    public string Email { get; init; } = null!;

    public string ConfirmationToken { get; init; } = null!;
}