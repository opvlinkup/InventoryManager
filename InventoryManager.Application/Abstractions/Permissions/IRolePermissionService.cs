using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Permissions;

public interface IRolePermissionService
{
    public Task AssignPermissionToRoleAsync(string roleName, string permissionName, CancellationToken ct);
    public Task<List<string>> GetPermissionsForUserAsync(User user, CancellationToken ct);
}