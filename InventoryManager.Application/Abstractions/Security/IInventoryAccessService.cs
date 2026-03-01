namespace InventoryManager.Application.Abstractions.Security;

public interface IInventoryAccessService
{
    Task EnsureCanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct);
    Task EnsureCanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct);
}