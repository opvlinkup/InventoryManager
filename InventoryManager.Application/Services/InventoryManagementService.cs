using System.Security.Cryptography;
using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Inventory;
using InventoryManager.Domain.Exceptions;
using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManager.Application.Services;

public sealed class InventoryManagementService(
    IUnitOfWork unitOfWork,
    IInventoryAccessService accessService,
    ILogger<InventoryManagementService> logger
) : IInventoryManagementService
{
   
    public async Task<Guid> CreateInventoryAsync(CreateInventoryDto dto, Guid ownerId, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title is required.");

            if (dto.Fields is null || dto.Fields.Count == 0)
                throw new ValidationException("At least one field is required.");

            var now = DateTime.UtcNow;

            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                ApiToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
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
            
            foreach (var field in dto.Fields)
            {
                if (string.IsNullOrWhiteSpace(field.DisplayName))
                    throw new ValidationException("Field display name is required.");

                if (field.Slot is <= 0 or > 3) 
                    throw new ValidationException($"Field slot '{field.Slot}' is out of allowed range.");
            }

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

            if (dto.CustomIdParts?.Count > 0)
            {
                var parts = dto.CustomIdParts
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

                await unitOfWork.CustomIdPartRepository.AddRangeAsync(parts, ct);
            }

            return inventory.Id;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error while creating inventory");

            throw new ValidationException("Invalid data provided. Check field constraints.");
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("CreateInventory operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating inventory");

            throw new Exception("Unexpected error occurred while creating inventory.");
        }
    }

    public async Task UpdateInventoryAsync(Guid inventoryId, UpdateInventoryDto dto, Guid userId, CancellationToken ct)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Title is required.");

            await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

            var inventory = await unitOfWork.InventoryRepository
                .GetForUpdateAsync(inventoryId, dto.RowVersion, ct);

            if (inventory is null)
                throw new NotFoundException("Inventory not found.");

            inventory.Title = dto.Title.Trim();
            inventory.Description = dto.Description?.Trim();
            inventory.IsPublic = dto.IsPublic;
            inventory.WriteAccessMode = dto.WriteAccessMode;
            inventory.UpdatedAt = DateTime.UtcNow;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrency conflict for inventory {InventoryId}", inventoryId);

            throw new DbConflictException("Inventory was modified by another user.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating inventory {InventoryId}", inventoryId);
            throw;
        }
    }
    
    
    public async Task DeleteInventoryAsync(Guid inventoryId, Guid userId, uint rowVersion, CancellationToken ct)
    {
        try
        {
            await accessService.EnsureCanEditAsync(inventoryId, userId, ct);

            var inventory = await unitOfWork.InventoryRepository
                .GetForUpdateAsync(inventoryId, rowVersion, ct);

            if (inventory is null)
                throw new NotFoundException("Inventory not found.");

            unitOfWork.InventoryRepository.Remove(inventory);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting inventory {InventoryId}", inventoryId);
            throw;
        }
    }
}