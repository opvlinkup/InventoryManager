namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface IUserRoleRepository
{
    Task<bool> IsAdminAsync(Guid userId, CancellationToken ct);
}