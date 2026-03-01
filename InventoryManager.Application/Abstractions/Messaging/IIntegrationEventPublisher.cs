namespace InventoryManager.Application.Abstractions.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default);
}