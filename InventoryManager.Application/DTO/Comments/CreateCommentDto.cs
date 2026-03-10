namespace InventoryManager.Application.DTO.Comments;

public class CreateCommentDto
{
    public Guid InventoryId { get; set; }
    public Guid UserId { get; set; }
    public string? Content { get; set; }
}