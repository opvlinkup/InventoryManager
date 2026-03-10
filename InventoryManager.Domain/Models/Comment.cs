using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;


public class Comment
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}