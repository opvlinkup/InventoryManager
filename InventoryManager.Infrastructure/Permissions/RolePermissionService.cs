using InventoryManager.Application.Abstractions.Permissions;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryManager.Infrastructure.Security;

public sealed class RolePermissionService(
    IUnitOfWork unitOfWork,
    RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager
) : IRolePermissionService
{
    public async Task AssignPermissionToRoleAsync(string roleName, string permissionName, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var role = await roleManager.FindByNameAsync(roleName)
                ?? throw new InvalidOperationException($"Role '{roleName}' not found");

            var permission = await unitOfWork.PermissionRepository.GetByAsync(
                p => p.Name == permissionName,
                ct);

            if (permission == null)
            {
                permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permissionName
                };

                await unitOfWork.PermissionRepository.AddAsync(permission, ct);
            }

            var exists = await unitOfWork.RolePermissionRepository.AnyAsync(
                rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id,
                ct);

            if (!exists)
            {
                await unitOfWork.RolePermissionRepository.AddAsync(
                    new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    },
                    ct);
            }

            await unitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<List<string>> GetPermissionsForUserAsync(User user, CancellationToken ct)
    {
        var roles = await userManager.GetRolesAsync(user);

        var permissions = new HashSet<string>();

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var rolePermissions =
                await unitOfWork.PermissionRepository
                    .GetPermissionNamesByRoleIdAsync(role.Id, ct);

            foreach (var p in rolePermissions)
                permissions.Add(p);
        }

        return permissions.ToList();
    }
}