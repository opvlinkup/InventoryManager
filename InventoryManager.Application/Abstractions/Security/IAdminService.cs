using InventoryManager.Application.DTO;
using InventoryManager.Application.DTO.Admin;
using InventoryManager.Application.DTO.User;

namespace InventoryManager.Application.Abstractions.Security;

public interface IAdminService
{
    Task<IReadOnlyList<UserAdminDto>> GetUsersAsync(UserFilterDto filter, CancellationToken ct);

    Task BlockAsync(Guid userId, Guid performedBy, CancellationToken ct);

    Task UnblockAsync(Guid userId, CancellationToken ct);

    Task GrantAdminAsync(Guid userId, CancellationToken ct);

    Task RevokeAdminAsync(Guid userId, Guid performedBy, CancellationToken ct);

    Task DeleteAsync(Guid userId, Guid performedBy, CancellationToken ct);
}