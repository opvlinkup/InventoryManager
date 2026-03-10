namespace InventoryManager.Application.Abstractions.Session;

public interface ISessionService
{
    public Task UpdateLastActivityAsync(Guid userId, CancellationToken ct);
}