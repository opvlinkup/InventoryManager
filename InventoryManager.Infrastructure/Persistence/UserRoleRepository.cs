using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;


public sealed class UserRoleRepository(InventoryManagerDbContext context)
    : IUserRoleRepository
{
    private Guid? _adminRoleId;

    public async Task<bool> IsAdminAsync(Guid userId, CancellationToken ct)
    {
        var adminRoleId = await GetAdminRoleIdAsync(ct);

        return await context.Set<IdentityUserRole<Guid>>()
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == adminRoleId, ct);
    }

    private async Task<Guid> GetAdminRoleIdAsync(CancellationToken ct)
    {
        if (_adminRoleId.HasValue)
            return _adminRoleId.Value;

        var roleId = await context.Set<IdentityRole<Guid>>()
            .Where(r => r.Name == "Admin")
            .Select(r => r.Id)
            .SingleAsync(ct);

        _adminRoleId = roleId;

        return roleId;
    }
}