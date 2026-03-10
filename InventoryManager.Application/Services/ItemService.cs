using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Fields;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Item;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public sealed class ItemService(
    IUnitOfWork unitOfWork,
    IInventoryAccessService accessService,
    IInventoryTemplateService templateService,
    IFieldMetadataService fieldMetadataService
) : IItemService
{
    public async Task<Guid> CreateItemAsync(Guid inventoryId, ItemDraftDto dto, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

        await templateService.ValidateItemAgainstTemplateAsync(
            inventoryId,
            dto,
            ct);

        var now = DateTime.UtcNow;

        var item = new Item
        {
            Id = Guid.NewGuid(),
            InventoryId = inventoryId,
            CreatedById = userId,
            CreatedAt = now,
            UpdatedAt = now,

            Text1 = dto.Text1?.Trim(),
            Text2 = dto.Text2?.Trim(),
            Text3 = dto.Text3?.Trim(),

            LongText1 = dto.LongText1?.Trim(),
            LongText2 = dto.LongText2?.Trim(),
            LongText3 = dto.LongText3?.Trim(),

            Number1 = dto.Number1,
            Number2 = dto.Number2,
            Number3 = dto.Number3,

            Bool1 = dto.Bool1,
            Bool2 = dto.Bool2,
            Bool3 = dto.Bool3,

            Link1 = dto.Link1?.Trim(),
            Link2 = dto.Link2?.Trim(),
            Link3 = dto.Link3?.Trim()
        };

        await unitOfWork.ItemRepository.AddAsync(item, ct);

        return item.Id;
    }

    public async Task UpdateItemAsync(
        Guid itemId,
        UpdateItemDto dto,
        Guid userId,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var item = await unitOfWork.ItemRepository
            .GetForUpdateAsync(itemId, dto.RowVersion, ct)
            ?? throw new InvalidOperationException("Item not found.");

        await accessService.EnsureCanEditAsync(item.InventoryId, userId, ct);

        var validationDraft = new ItemDraftDto
        {
            Text1 = dto.Text1,
            Text2 = dto.Text2,
            Text3 = dto.Text3,

            LongText1 = dto.LongText1,
            LongText2 = dto.LongText2,
            LongText3 = dto.LongText3,

            Number1 = dto.Number1,
            Number2 = dto.Number2,
            Number3 = dto.Number3,

            Bool1 = dto.Bool1,
            Bool2 = dto.Bool2,
            Bool3 = dto.Bool3,

            Link1 = dto.Link1,
            Link2 = dto.Link2,
            Link3 = dto.Link3
        };

        await templateService.ValidateItemAgainstTemplateAsync(item.InventoryId, validationDraft, ct);

        item.Text1 = dto.Text1?.Trim();
        item.Text2 = dto.Text2?.Trim();
        item.Text3 = dto.Text3?.Trim();

        item.LongText1 = dto.LongText1?.Trim();
        item.LongText2 = dto.LongText2?.Trim();
        item.LongText3 = dto.LongText3?.Trim();

        item.Number1 = dto.Number1;
        item.Number2 = dto.Number2;
        item.Number3 = dto.Number3;

        item.Bool1 = dto.Bool1;
        item.Bool2 = dto.Bool2;
        item.Bool3 = dto.Bool3;

        item.Link1 = dto.Link1?.Trim();
        item.Link2 = dto.Link2?.Trim();
        item.Link3 = dto.Link3?.Trim();

        item.UpdatedAt = DateTime.UtcNow;
    }

    public async Task DeleteItemAsync(
        Guid itemId,
        byte[] rowVersion,
        Guid userId,
        CancellationToken ct)
    {
        var item = await unitOfWork.ItemRepository
            .GetForUpdateAsync(itemId, rowVersion, ct)
            ?? throw new InvalidOperationException("Item not found.");

        await accessService.EnsureCanEditAsync(item.InventoryId, userId, ct);

        unitOfWork.ItemRepository.Remove(item);
    }
    
    public async Task<List<ItemTableDto>> GetByInventoryAsync(Guid inventoryId, ItemFilterDto filter, CancellationToken ct)
    {
        var inventory = await unitOfWork.InventoryRepository.GetByIdAsync(inventoryId, ct);

        if (inventory == null)
            throw new Exception("Inventory not found");

        var fields = inventory.FieldDefinitions
            .Where(f => f.State == FieldState.Required && f.ShowInUiTable)
            .OrderBy(f => f.Order)
            .ToList();

        var items = await unitOfWork.ItemRepository.GetByInventoryIdAsync(inventoryId, ct);

        var result = new List<ItemTableDto>();

        foreach (var item in items)
        {
            var dto = new ItemTableDto
            {
                Id = item.Id,
                CustomId = item.CustomId,
                CreatedAt = item.CreatedAt
            };

            foreach (var field in fields)
            {
                dto.Fields[field.DisplayName] = fieldMetadataService.GetItemFieldValue(item, field);
            }

            result.Add(dto);
        }

        return result;
    }
}