using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public sealed class RolePermissionRepository(InventoryManagerDbContext context) : Repository<RolePermission>(context), IRolePermissionRepository
{
}