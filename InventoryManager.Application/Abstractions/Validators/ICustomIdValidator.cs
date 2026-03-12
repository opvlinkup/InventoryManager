namespace InventoryManager.Application.Abstractions.Validators;

public interface ICustomIdValidator
{
    Task<long?> ValidateAsync(Guid inventoryId, string customId, CancellationToken ct);
}