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
            WriteAccessMode = dto.WriteAccessMode,
            CreatedAt = now,
            UpdatedAt = now
        };

        await unitOfWork.InventoryRepository.AddAsync(inventory, ct);

        return inventory.Id;
    }

    public async Task UpdateInventoryAsync(Guid inventoryId, UpdateInventoryDto dto, Guid userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(dto.RowVersion);

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

    public async Task DeleteInventoryAsync(Guid inventoryId, Guid userId, byte[] rowVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(rowVersion);

        await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

        var inventory = await unitOfWork.InventoryRepository
            .GetForUpdateAsync(inventoryId, rowVersion, ct);

        unitOfWork.InventoryRepository.Remove(inventory);
    }
}