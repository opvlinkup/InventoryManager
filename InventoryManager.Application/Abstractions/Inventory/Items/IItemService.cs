using InventoryManager.Application.DTO.Item;

namespace InventoryManager.Application.Abstractions.Inventory.Items;

public interface IItemService
{
    Task<Guid> CreateItemAsync(ItemDraftDto draftDto, Guid userId, CancellationToken ct);

    Task UpdateItemAsync(Guid itemId, UpdateItemDto dto, Guid userId, CancellationToken ct);

    public Task DeleteItemAsync(
        Guid itemId,
        uint rowVersion,
        Guid userId,
        CancellationToken ct);

    public Task<List<ItemTableDto>> GetByInventoryAsync(Guid inventoryId, ItemFilterDto filter, CancellationToken ct);
}
