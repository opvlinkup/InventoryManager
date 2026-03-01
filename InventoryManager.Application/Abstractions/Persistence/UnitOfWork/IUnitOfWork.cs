using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    ISessionRepository SessionRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IInventoryRepository InventoryRepository { get; }
    IInventoryWriteAccessRepository InventoryWriteAccessRepository { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    ValueTask DisposeAsync();
    void Dispose();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}