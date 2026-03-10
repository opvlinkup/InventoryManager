using InventoryManager.Application.DTO.Comments;

namespace InventoryManager.Application.Abstractions.Inventory.Comments;

public interface IDiscussionService
{
    Task<IReadOnlyList<CommentDto>> GetAsync(Guid inventoryId, CancellationToken ct);
    Task AddAsync(Guid inventoryId, CreateCommentDto dto, Guid userId, CancellationToken ct);
}