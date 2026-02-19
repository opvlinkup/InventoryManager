using System.ComponentModel.DataAnnotations;

namespace InventoryManager.Domain.Models;

public class CustomIdPart
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;
    
    public CustomIdPartType Type { get; set; }

    public int Order { get; set; }

    [MaxLength(200)]
    public string? Format { get; set; }

    [MaxLength(200)]
    public string? FixedValue { get; set; }
}