using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class InventoryRepository(InventoryManagerDbContext context) : Repository<Inventory>(context), IInventoryRepository
{
    public async Task<InventoryAccessSnapshot?> GetAccessSnapshotAsync(Guid inventoryId, CancellationToken ct)
    {
        return await context.Inventories
            .Where(i => i.Id == inventoryId)
            .Select(i => new InventoryAccessSnapshot(
                i.OwnerId,
                i.IsPublic,
                i.WriteAccessMode))
            .FirstOrDefaultAsync(ct);
    }
}