using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IItemRepository : IRepository<Item>
{
    Task<IReadOnlyList<Item>> GetByInventoryAsync(Guid inventoryId, CancellationToken ct);
}