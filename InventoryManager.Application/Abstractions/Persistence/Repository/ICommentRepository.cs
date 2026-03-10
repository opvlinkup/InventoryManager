using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.DTO.Comments;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface ICommentRepository
{
    Task<IReadOnlyList<CommentDto>> GetCommentAsync(Guid inventoryId, CancellationToken ct);

    Task<CommentDto> AddCommentAsync(Guid inventoryId, Guid userId, string content, CancellationToken ct);
}
