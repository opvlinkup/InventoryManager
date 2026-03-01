namespace InventoryManager.Application.Abstractions.Messaging;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}