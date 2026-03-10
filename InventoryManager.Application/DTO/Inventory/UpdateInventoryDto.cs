using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Inventory;
public sealed class UpdateInventoryDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public WriteAccessMode WriteAccessMode { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}