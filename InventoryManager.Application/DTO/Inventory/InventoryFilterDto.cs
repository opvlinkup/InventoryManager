namespace InventoryManager.Application.DTO.Inventory;

public class InventoryFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public Guid? OwnerId { get; set; }
    public Guid? CategoryId { get; set; }

    public string? SortBy { get; set; }
    public bool Desc { get; set; }
}
