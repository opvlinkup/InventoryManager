using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<InventoryTag> Inventories { get; set; } = new List<InventoryTag>();
}