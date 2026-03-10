using InventoryManager.Application.Abstractions.Inventory.Comments;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.DTO.Comments;

namespace InventoryManager.Application.Services;

public sealed class DiscussionService(IUnitOfWork unitOfWork ) : IDiscussionService
{
    public Task<IReadOnlyList<CommentDto>> GetAsync(Guid inventoryId, CancellationToken ct)
    {
        return unitOfWork.CommentRepository.GetCommentAsync(inventoryId, ct);
    }

    public async Task AddAsync(Guid inventoryId, CreateCommentDto dto, Guid userId, CancellationToken ct)
    {
        var inventoryExists =
            await unitOfWork.InventoryRepository.AnyAsync(i => i.Id == inventoryId, ct);

        if (!inventoryExists)
            throw new Exception("Inventory not found");

        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new Exception("Comment cannot be empty");

        var content = dto.Content.Trim();

        if (content.Length > 1000)
            throw new Exception("Comment cannot be longer than 1000 characters");

        if (content.Length < 1)
            throw new Exception("Comment must be at least 1 character long");

        await unitOfWork.CommentRepository.AddCommentAsync(inventoryId, userId, content, ct);
    }
}