using InventoryManager.Application.DTO.Inventory;

namespace InventoryManager.Application.Abstractions.Inventory;

    public interface IInventoryManagementService
    {
        Task<Guid> CreateInventoryAsync(CreateInventoryDto dto, Guid ownerId, CancellationToken ct);

        Task UpdateInventoryAsync(Guid inventoryId, UpdateInventoryDto dto, Guid userId, CancellationToken ct);

        public Task DeleteInventoryAsync(Guid inventoryId, Guid userId, uint rowVersion, CancellationToken ct);
    }
