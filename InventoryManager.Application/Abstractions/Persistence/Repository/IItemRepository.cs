using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> GetForUpdateAsync(Guid id, byte[] rowVersion, CancellationToken ct);
    Task<List<Item>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
}