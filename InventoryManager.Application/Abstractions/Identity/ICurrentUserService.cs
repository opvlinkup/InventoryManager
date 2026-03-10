namespace InventoryManager.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    public Task UpdateLastActivityAsync(CancellationToken ct);
}