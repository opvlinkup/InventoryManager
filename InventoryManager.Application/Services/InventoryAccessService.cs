using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Application.Services;

public sealed class InventoryAccessService(IUnitOfWork unitOfWork, UserManager<User> userManager ) : IInventoryAccessService
{
    public async Task EnsureCanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        var inventorySnapshot = await unitOfWork.InventoryRepository
                           .GetAccessSnapshotAsync(inventoryId, ct)
                       ?? throw new InvalidOperationException("Inventory not found");

        if (await IsAdminAsync(userId))
            return;

        if (inventorySnapshot.OwnerId == userId)
            return;

        if (inventorySnapshot.IsPublic)
            return;

        var hasAccess = await unitOfWork.InventoryWriteAccessRepository
            .HasWriteAccessAsync(inventoryId, userId, ct);

        if (!hasAccess)
            throw new UnauthorizedAccessException("Access denied");
    }

    public async Task EnsureCanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        var inventorySnapshot = await unitOfWork.InventoryRepository
                           .GetAccessSnapshotAsync(inventoryId, ct)
                       ?? throw new InvalidOperationException("Inventory not found");

        if (await IsAdminAsync(userId))
            return;

        if (inventorySnapshot.OwnerId == userId)
            return;

        if (inventorySnapshot.WriteAccessMode == WriteAccessMode.AllAuthenticatedUsers)
            return;

        var hasWriteAccess = await unitOfWork.InventoryWriteAccessRepository
            .HasWriteAccessAsync(inventoryId, userId, ct);

        if (!hasWriteAccess)
            throw new UnauthorizedAccessException("Edit access denied");
    }

    private async Task<bool> IsAdminAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new UnauthorizedAccessException();

        return await userManager.IsInRoleAsync(user, "Admin");
    }
}