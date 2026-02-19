using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManager.Domain.Models;

public class FieldMetadata
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }
    [ForeignKey(nameof(InventoryId))]
    public Inventory Inventory { get; set; } = null!;

    public FieldType Type { get; set; }
    public FieldState State { get; set; }
    public int Slot { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string DisplayName { get; set; } = null!;

    [MaxLength(400)]
    public string? Tooltip { get; set; }
    
    public bool ShowInUiTable { get; set; }
    public int Order { get; set; }
}