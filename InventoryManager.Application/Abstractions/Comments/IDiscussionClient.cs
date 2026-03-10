using InventoryManager.Application.DTO.Comments;

namespace InventoryManager.Application.Abstractions.Comments;

public interface IDiscussionClient
{
    Task ReceiveComment(CommentDto comment);
}