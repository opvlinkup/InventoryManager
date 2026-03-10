using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.DTO.Comments;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using InventoryManager.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public sealed class CommentRepository(InventoryManagerDbContext context, IHubContext<DiscussionHub> hubContext)
    : ICommentRepository
{
    public async Task<IReadOnlyList<CommentDto>> GetCommentAsync(Guid inventoryId, CancellationToken ct)
    {
        return await context.Set<Comment>()
            .Where(c => c.InventoryId == inventoryId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                Name = c.User.Name,
                UserId = c.User.Id,
                CreatedAt = c.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<CommentDto> AddCommentAsync(Guid inventoryId, Guid userId, string content, CancellationToken ct)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            InventoryId = inventoryId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        context.Set<Comment>().Add(comment);

        await context.SaveChangesAsync(ct);

        var dto = await context.Set<Comment>()
            .Where(c => c.Id == comment.Id)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                Name = c.User.Name,
                UserId = c.User.Id,
                CreatedAt = c.CreatedAt
            })
            .SingleAsync(ct);

        await hubContext.Clients.Group(inventoryId.ToString()).SendAsync("ReceiveComment", dto, ct);

        return dto;
    }
}