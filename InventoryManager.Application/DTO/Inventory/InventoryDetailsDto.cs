namespace InventoryManager.Application.DTO.Inventory;

public sealed class InventoryDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}