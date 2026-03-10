using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class ItemRepository(InventoryManagerDbContext context) : Repository<Item>(context), IItemRepository
{
    public async Task<bool> ExistsAsync(Guid itemId, CancellationToken ct)
    {
        return await context.Items.AnyAsync(x => x.Id == itemId, ct);
    }
    
    public async Task<Item?> GetForUpdateAsync(Guid id, byte[] rowVersion, CancellationToken ct)
    {
        var entity = await context.Items.FirstOrDefaultAsync(i => i.Id == id, ct);

        if (entity == null)
            return null;

        context.SetOriginalConcurrencyToken(entity, x => x.RowVersion, rowVersion);

        return entity;
    }
    public async Task<List<Item>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken ct)
    {
        return await context.Items.Where(x => x.InventoryId == inventoryId).AsNoTracking().ToListAsync(ct);
    }
}