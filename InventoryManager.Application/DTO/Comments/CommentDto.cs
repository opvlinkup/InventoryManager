namespace InventoryManager.Application.DTO.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Name { get; set; }
    public Guid UserId { get; set; }
    
}