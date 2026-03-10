namespace InventoryManager.Application.Abstractions.Security;

public interface IInventoryAccessService
{
  
    Task<bool> CanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct);

    Task<bool> CanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct);

    Task EnsureCanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct);

    Task EnsureCanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct);
}