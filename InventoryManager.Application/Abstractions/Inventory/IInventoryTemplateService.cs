using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Application.DTO.Item;

namespace InventoryManager.Application.Abstractions.Inventory;

public interface IInventoryTemplateService
{
    Task<IReadOnlyList<FieldRealMetadataDto>> GetTemplateAsync(
        Guid inventoryId,
        CancellationToken ct);

    Task UpdateTemplateAsync(Guid inventoryId, IReadOnlyList<FieldInitialMetadataDto> fields, Guid userId,
        CancellationToken ct);
    

    Task ValidateItemAgainstTemplateAsync(
        Guid inventoryId,
        ItemDraftDto item,
        CancellationToken ct);
}