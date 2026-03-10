using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Application.Services;

public sealed class LikeService(IUnitOfWork unitOfWork ) : ILikeService
{
    public async Task LikeAsync(Guid itemId, Guid userId, CancellationToken ct)
    {
        var exists = await unitOfWork.ItemRepository.ExistsAsync(itemId, ct);

        if (!exists)
            throw new InvalidOperationException("Item not found");

        var alreadyLiked = await unitOfWork.LikeRepository.ExistsAsync(itemId, userId, ct);

        if (alreadyLiked)
            return;

        var like = new Like
        {
            ItemId = itemId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.LikeRepository.AddLikeAsync(like, ct);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Failed to like the item. It may have been liked by another user.");
        }
    }

    public async Task UnlikeAsync(Guid itemId, Guid userId, CancellationToken ct)
    {
        await unitOfWork.LikeRepository.RemoveLikeAsync(itemId, userId, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }
}