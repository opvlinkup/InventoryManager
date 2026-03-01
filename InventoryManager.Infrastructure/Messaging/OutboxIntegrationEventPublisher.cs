using System.Text.Json;
using InventoryManager.Application.Abstractions.Messaging;
using InventoryManager.Infrastructure.Database;
using InventoryManager.Infrastructure.Email;

namespace InventoryManager.Infrastructure.Messaging;

public class OutboxIntegrationEventPublisher(InventoryManagerDbContext context) : IIntegrationEventPublisher
{
    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken ct = default)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().FullName!,
            Content = JsonSerializer.Serialize(integrationEvent),
            OccurredOn = integrationEvent.OccurredOn
        };

        await context.OutboxMessages.AddAsync(outboxMessage, ct);
    }
}