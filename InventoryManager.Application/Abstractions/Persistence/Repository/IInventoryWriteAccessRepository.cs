namespace InventoryManager.Application.Abstractions.Persistence;

public interface IInventoryWriteAccessRepository
{
    public Task<bool> HasWriteAccessAsync(Guid inventoryId, Guid userId, CancellationToken ct);
}