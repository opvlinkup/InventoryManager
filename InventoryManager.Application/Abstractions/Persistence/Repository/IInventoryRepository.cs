using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IInventoryRepository : IRepository<Inventory>
{
   public Task<InventoryAccessSnapshot?> GetAccessSnapshotAsync(Guid inventoryId, CancellationToken ct);
}