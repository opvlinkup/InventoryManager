using InventoryManager.Domain.Models;

namespace InventoryManager.Application.DTO.Inventory;

public class CustomIdPartDto
{
    public CustomIdPartType Type { get; set; }

    public string? FixedValue { get; set; }

    public string? Format { get; set; }

    public int Order { get; set; }
}