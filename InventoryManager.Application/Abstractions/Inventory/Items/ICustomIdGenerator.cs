namespace InventoryManager.Application.Abstractions.Inventory.Items;

public interface ICustomIdGenerator
{
    Task<(string customId, long? sequence)> GenerateAsync(Guid inventoryId, CancellationToken ct);
}