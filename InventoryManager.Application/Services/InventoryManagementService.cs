using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public sealed class InventoryManagementService(
    IUnitOfWork unitOfWork,
    IInventoryAccessService accessService
) : IInventoryManagementService
{
    public async Task<Guid> CreateInventoryAsync(CreateInventoryDto dto, Guid ownerId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        var now = DateTime.UtcNow;

        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl?.Trim(),
            IsPublic = dto.IsPublic,
            WriteAccessMode = WriteAccessMode.OwnerOnly,
            CreatedAt = now,
            UpdatedAt = now
        };
        
        
        await unitOfWork.InventoryRepository.AddAsync(inventory, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var fieldDefinitions = dto.Fields
            .OrderBy(f => f.Order)
            .Select(f => new FieldMetadata
            {
                Id = Guid.NewGuid(),
                InventoryId = inventory.Id,
                Type = f.Type,
                State = f.State,
                Slot = f.Slot,
                DisplayName = f.DisplayName.Trim(),
                Tooltip = f.Tooltip?.Trim(),
                ShowInUiTable = f.ShowInUiTable,
                Order = f.Order
            })
            .ToList();

        await unitOfWork.FieldMetadataRepository.AddRangeAsync(fieldDefinitions, ct);
        if (dto.CustomIdParts.Count > 0)
        {
            var customIdParts = dto.CustomIdParts
                .OrderBy(p => p.Order)
                .Select(p => new CustomIdPart
                {
                    Id = Guid.NewGuid(),
                    InventoryId = inventory.Id,
                    Type = p.Type,
                    FixedValue = p.FixedValue,
                    Format = p.Format,
                    Order = p.Order
                })
                .ToList();
            
                await unitOfWork.CustomIdPartRepository.AddRangeAsync(customIdParts, ct);
            
        }
        
        return inventory.Id;
    }

    public async Task UpdateInventoryAsync(Guid inventoryId, UpdateInventoryDto dto, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

        var inventory = await unitOfWork.InventoryRepository
            .GetForUpdateAsync(inventoryId, dto.RowVersion, ct);

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required.");

        inventory.Title = dto.Title.Trim();
        inventory.Description = dto.Description?.Trim();
        inventory.IsPublic = dto.IsPublic;
        inventory.WriteAccessMode = dto.WriteAccessMode;
        inventory.UpdatedAt = DateTime.UtcNow;
    }

    public async Task DeleteInventoryAsync(Guid inventoryId, Guid userId, uint rowVersion, CancellationToken ct)
    {
        
        await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

        var inventory = await unitOfWork.InventoryRepository
            .GetForUpdateAsync(inventoryId, rowVersion, ct);

        unitOfWork.InventoryRepository.Remove(inventory);
    }
}