using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Inventory;

public sealed class CreateInventoryDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsPublic { get; set; }
    
    public WriteAccessMode WriteAccessMode { get; set; }

    public IReadOnlyList<FieldInitialMetadataDto> Fields { get; set; } = [];
}