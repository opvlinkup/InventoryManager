using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO;
using InventoryManager.Application.DTO.Admin;
using InventoryManager.Application.DTO.User;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Application.Services;

public sealed class AdminService(IUnitOfWork unitOfWork) : IAdminService
{
    public Task<IReadOnlyList<UserAdminDto>> GetUsersAsync(UserFilterDto filter, CancellationToken ct)
    {
        return unitOfWork.AdminRepository.GetUsersAsync(filter, ct);
    }

    public async Task BlockAsync(Guid userId, Guid performedBy, CancellationToken ct)
    {
        if (userId == performedBy)
            throw new InvalidOperationException("Admin cannot block themselves.");

        var user = await unitOfWork.AdminRepository.GetUserByIdAsync(userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        if (user.Status == Status.Blocked)
            return;

        user.Status = Status.Blocked;

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.AdminRepository.InvalidateSessionsAsync(userId, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UnblockAsync(Guid userId, CancellationToken ct)
    {
        var user = await unitOfWork.AdminRepository.GetUserByIdAsync(userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        if (user.Status != Status.Blocked)
            return;

        user.Status = Status.Active;

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task GrantAdminAsync(Guid userId, CancellationToken ct)
    {
        var user = await unitOfWork.AdminRepository.GetUserByIdAsync(userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        await unitOfWork.AdminRepository.GrantAdminAsync(user);
    }

    public async Task RevokeAdminAsync(Guid userId, Guid performedBy, CancellationToken ct)
    {
        var user = await unitOfWork.AdminRepository.GetUserByIdAsync(userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        await unitOfWork.AdminRepository.RevokeAdminAsync(user);
    }

    public async Task DeleteAsync(Guid userId, Guid performedBy, CancellationToken ct)
    {
        if (userId == performedBy)
            throw new InvalidOperationException("Admin cannot delete themselves.");

        var user = await unitOfWork.AdminRepository.GetUserByIdAsync(userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        await unitOfWork.AdminRepository.DeleteUserAsync(user);
    }
}