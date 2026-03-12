using InventoryManager.Application.Abstractions.Inventory.Comments;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManager.Application.Abstractions.Persistence.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    ISessionRepository SessionRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IInventoryRepository InventoryRepository { get; }
    IInventoryWriteAccessRepository InventoryWriteAccessRepository { get; }
    IFieldMetadataRepository FieldMetadataRepository { get; }
    IItemRepository ItemRepository { get; }
    ILikeRepository LikeRepository { get; }
    IAdminRepository AdminRepository { get; }
    ICommentRepository CommentRepository { get; }
    IUserRoleRepository UserRoleRepository { get; }
    ICustomIdPartRepository CustomIdPartRepository { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    ValueTask DisposeAsync();
    void Dispose();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}