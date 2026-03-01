using Microsoft.AspNetCore.Identity;

namespace InventoryManager.Domain.Models;

public class RolePermission
{
    public string RoleId { get; set; }
    public IdentityRole Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}