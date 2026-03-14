using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IItemRepository : IRepository<Item>
{
    public Task<Item?> GetForUpdateAsync(Guid id, uint rowVersion, CancellationToken ct);
    Task<List<Item>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    public Task<long> GetNextSequenceAsync(Guid inventoryId, CancellationToken ct);
}