using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Services;

public sealed class InventoryAccessService(IUnitOfWork unitOfWork, IUserRoleRepository roleRepository) : IInventoryAccessService
{
    public async Task<bool> CanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        var snapshot = await unitOfWork.InventoryRepository
            .GetAccessSnapshotAsync(inventoryId, ct);

        if (snapshot is null)
            return false;

        if (snapshot.IsPublic)
            return true;

        if (snapshot.OwnerId == userId)
            return true;

        if (await roleRepository.IsAdminAsync(userId, ct))
            return true;

        return false;
    }

    public async Task<bool> CanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        var snapshot = await unitOfWork.InventoryRepository
            .GetAccessSnapshotAsync(inventoryId, ct);

        if (snapshot is null)
            return false;

        if (snapshot.OwnerId == userId)
            return true;

        if (await roleRepository.IsAdminAsync(userId, ct))
            return true;

        if (snapshot.WriteAccessMode == WriteAccessMode.AllAuthenticatedUsers)
            return true;

        return await unitOfWork.InventoryWriteAccessRepository
            .HasWriteAccessAsync(inventoryId, userId, ct);
    }

    public async Task EnsureCanViewAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        if (!await CanViewAsync(inventoryId, userId, ct))
            throw new UnauthorizedAccessException("Access denied");
    }

    public async Task EnsureCanEditAsync(Guid inventoryId, Guid userId, CancellationToken ct)
    {
        if (!await CanEditAsync(inventoryId, userId, ct))
            throw new UnauthorizedAccessException("Edit access denied");
    }
}