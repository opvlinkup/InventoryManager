using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace InventoryManager.Infrastructure.Persistence;

public class LikeRepository(InventoryManagerDbContext context) : Repository<Like>(context), ILikeRepository
{
    public async Task<bool> ExistsAsync(Guid itemId, Guid userId, CancellationToken ct)
    {
        return await context.Likes.AnyAsync(x => x.ItemId == itemId && x.UserId == userId, ct);
    }

    public async Task AddLikeAsync(Like like, CancellationToken ct)
    {
        await context.Likes.AddAsync(like, ct);
    }

    public async Task RemoveLikeAsync(Guid itemId, Guid userId, CancellationToken ct)
    {
        await context.Likes.Where(x => x.ItemId == itemId && x.UserId == userId).ExecuteDeleteAsync(ct);
    }
}