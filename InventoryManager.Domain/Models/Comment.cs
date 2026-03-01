using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;


public class Comment
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}