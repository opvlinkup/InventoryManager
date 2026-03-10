using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Fields;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Application.DTO.Item;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public sealed class InventoryTemplateService(
    IUnitOfWork unitOfWork,
    IInventoryAccessService accessService,
    IFieldMetadataService fieldMetadataService
) : IInventoryTemplateService
{
    public async Task<IReadOnlyList<FieldRealMetadataDto>> GetTemplateAsync(Guid inventoryId, CancellationToken ct)
    {
        var fields = await unitOfWork.FieldMetadataRepository.GetManyByAsync(
            filter: f => f.InventoryId == inventoryId,
            orderBy: q => q.OrderBy(f => f.Order),
            include: null,
            isTrackingEnabled: false,
            cancellationToken: ct);

        return fields
            .Select(f => new FieldRealMetadataDto
            {
                Id = f.Id,
                Type = f.Type,
                State = f.State,
                Slot = f.Slot,
                DisplayName = f.DisplayName,
                Tooltip = f.Tooltip,
                ShowInUiTable = f.ShowInUiTable,
                Order = f.Order
            })
            .ToList();
    }

    public async Task UpdateTemplateAsync(Guid inventoryId, IReadOnlyList<FieldInitialMetadataDto> fields, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(fields);

        await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

        ValidateTemplate(fields);

        var existing = await unitOfWork.FieldMetadataRepository.GetManyByAsync(
            f => f.InventoryId == inventoryId,
            cancellationToken: ct);

        unitOfWork.FieldMetadataRepository.RemoveRange(existing);

        foreach (var field in fields)
        {
            var entity = new FieldMetadata
            {
                Id = Guid.NewGuid(),
                InventoryId = inventoryId,
                Type = field.Type,
                State = field.State,
                Slot = field.Slot,
                DisplayName = field.DisplayName.Trim(),
                Tooltip = field.Tooltip?.Trim(),
                ShowInUiTable = field.ShowInUiTable,
                Order = field.Order
            };

            await unitOfWork.FieldMetadataRepository.AddAsync(entity, ct);
        }
    }

    public async Task ValidateItemAgainstTemplateAsync(Guid inventoryId, ItemDraftDto item, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(item);

        var template = await unitOfWork.FieldMetadataRepository.GetManyByAsync(
            f => f.InventoryId == inventoryId,
            cancellationToken: ct);

        foreach (var field in template)
        {
            if (field.State != FieldState.Required)
                continue;

            if (!fieldMetadataService.ItemHasFieldValue(item, field.Type, field.Slot))
            {
                throw new InvalidOperationException(
                    $"Required field '{field.DisplayName}' (type: {field.Type}, slot: {field.Slot}) is missing.");
            }
        }
    }


    private static void ValidateTemplate(IReadOnlyList<FieldInitialMetadataDto> fields)
    {
        var usedSlots = new HashSet<(FieldType Type, int Slot)>();

        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.DisplayName))
                throw new ArgumentException("Field DisplayName is required.");

            if (field.Slot <= 0 || field.Slot > 3)
                throw new ArgumentOutOfRangeException(nameof(field.Slot));

            if (field.Order < 0 || field.Order > fields.Count)
                throw new ArgumentOutOfRangeException(nameof(field.Order));

            if (!usedSlots.Add((field.Type, field.Slot)))
                throw new InvalidOperationException(
                    $"Duplicate slot for type '{field.Type}' and slot '{field.Slot}'.");
        }
    }
}
    