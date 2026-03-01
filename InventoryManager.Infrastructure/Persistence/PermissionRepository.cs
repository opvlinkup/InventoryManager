using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class PermissionRepository(InventoryManagerDbContext context) : Repository<Permission>(context), IPermissionRepository
{
    
    public async Task<List<string>> GetPermissionNamesByRoleIdAsync(string roleId, CancellationToken ct = default)
    {
        return await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .AsNoTracking()
            .ToListAsync(ct);
    }
}