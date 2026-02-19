namespace InventoryManager.Domain.Models;

public class Like
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
}