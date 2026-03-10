using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface ILikeRepository
{
    Task<bool> ExistsAsync(Guid itemId, Guid userId, CancellationToken ct);

    Task AddLikeAsync(Like like, CancellationToken ct);

    Task RemoveLikeAsync(Guid itemId, Guid userId, CancellationToken ct);
}