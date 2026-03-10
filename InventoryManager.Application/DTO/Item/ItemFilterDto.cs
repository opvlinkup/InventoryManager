namespace InventoryManager.Application.DTO.Item;


public sealed class ItemFilterDto
{
    public string? Search { get; init; }

    public string? SortBy { get; init; }

    public bool Desc { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}