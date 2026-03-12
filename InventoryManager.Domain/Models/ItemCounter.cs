namespace InventoryManager.Domain.Models;

public class ItemCounter
{
    public Guid InventoryId { get; set; }

    public long NextValue { get; set; }

    public Inventory Inventory { get; set; } = null!;
}