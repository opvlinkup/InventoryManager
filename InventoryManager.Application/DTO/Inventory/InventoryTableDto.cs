namespace InventoryManager.Application.DTO.Inventory;

public sealed class InventoryTableDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string OwnerName { get; set; } = default!;
    public int ItemsCount { get; set; }
}