namespace InventoryManager.Application.Abstractions.Inventory.Likes;

public interface ILikeService
{
    Task LikeAsync(Guid itemId, Guid userId, CancellationToken ct);

    Task UnlikeAsync(Guid itemId, Guid userId, CancellationToken ct);
}