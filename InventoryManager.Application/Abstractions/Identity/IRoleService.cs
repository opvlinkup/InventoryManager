using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Identity;

public interface IRoleService
{
    Task<bool> IsAdminAsync(Guid userId);
    Task EnsurePermissionExistsAsync(string permissionName, CancellationToken cancellationToken = default);
    Task<List<string>> GetPermissionsForRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task AssignPermissionToRoleAsync(string permissionName, string roleName,
        CancellationToken cancellationToken = default);
    Task AssignPermissionsToUserAsync(User user, string roleName, CancellationToken cancellationToken = default);
}