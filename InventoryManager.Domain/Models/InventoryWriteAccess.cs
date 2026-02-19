namespace InventoryManager.Domain.Models;

public class InventoryWriteAccess
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;
}