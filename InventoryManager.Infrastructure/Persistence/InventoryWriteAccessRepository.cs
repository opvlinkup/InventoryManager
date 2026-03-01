using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class InventoryWriteAccessRepository(InventoryManagerDbContext context) : Repository<InventoryWriteAccess>(context), IInventoryWriteAccessRepository
{
    public Task<bool> HasWriteAccessAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        return context.InventoryWriteAccesses
            .AnyAsync(a =>
                    a.InventoryId == inventoryId &&
                    a.UserId == userId,
                ct);
    }
}