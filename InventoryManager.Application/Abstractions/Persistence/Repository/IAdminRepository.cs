using InventoryManager.Application.DTO.Admin;
using InventoryManager.Application.DTO.User;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface IAdminRepository
{
    Task<IReadOnlyList<UserAdminDto>> GetUsersAsync(UserFilterDto filter, CancellationToken ct);

    Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct);

    Task<bool> IsUserAdminAsync(Guid userId, CancellationToken ct);

    Task GrantAdminAsync(User user);

    Task RevokeAdminAsync(User user);

    Task DeleteUserAsync(User user);

    Task InvalidateSessionsAsync(Guid userId, CancellationToken ct);
}