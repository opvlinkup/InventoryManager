using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Inventory;

public sealed class FieldInitialMetadataDto
{
    public FieldType Type { get; set; }
    public FieldState State { get; set; }
    public int Slot { get; set; }

    public string DisplayName { get; set; } = null!;
    public string? Tooltip { get; set; }

    public bool ShowInUiTable { get; set; }
    public int Order { get; set; }
}