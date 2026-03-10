namespace InventoryManager.Application.DTO.Item;

public class ItemTableDto
{
    public Guid Id { get; set; }

    public string CustomId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Dictionary<string, object?> Fields { get; set; } = new();
}