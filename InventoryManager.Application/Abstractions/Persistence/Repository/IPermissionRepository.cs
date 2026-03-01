using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IPermissionRepository : IRepository<Permission>
{
        Task<List<string>> GetPermissionNamesByRoleIdAsync(string roleId, CancellationToken ct = default);
}