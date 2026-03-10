using InventoryManager.Application.Abstractions.Inventory.Comments;
using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManager.Infrastructure.Persistence;

public sealed class UnitOfWork(
    InventoryManagerDbContext dbContext,
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPermissionRepository permissionRepository,
    IRolePermissionRepository rolePermissionRepository,
    IInventoryRepository inventoryRepository,
    IInventoryWriteAccessRepository inventoryWriteAccessRepository,
    IFieldMetadataRepository fieldMetadataRepository,
    IItemRepository itemRepository,
    ILikeRepository likeRepository,
    IAdminRepository adminRepository,
    ICommentRepository commentRepository,
    IUserRoleRepository userRoleRepository)
    : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly InventoryManagerDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public IUserRepository UserRepository { get; } = userRepository;
    public ISessionRepository SessionRepository { get; } = sessionRepository;
    public IPermissionRepository PermissionRepository { get; } = permissionRepository;
    public IRolePermissionRepository RolePermissionRepository { get; } = rolePermissionRepository;
    public IInventoryRepository InventoryRepository { get; } = inventoryRepository;
    public IInventoryWriteAccessRepository InventoryWriteAccessRepository { get; } = inventoryWriteAccessRepository;
    public IFieldMetadataRepository FieldMetadataRepository { get; } = fieldMetadataRepository;
    public IItemRepository ItemRepository { get; } = itemRepository;
    public ILikeRepository LikeRepository { get; } = likeRepository;
    public IAdminRepository AdminRepository { get; } = adminRepository;
    public ICommentRepository CommentRepository { get; } = commentRepository;
    public IUserRoleRepository UserRoleRepository { get; } = userRoleRepository;
 
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return _currentTransaction;

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }
    }

    public async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await DisposeTransactionAsync();
        await _dbContext.DisposeAsync();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
    
    public void Dispose()
    {
        if (_disposed) return;

        _currentTransaction?.Dispose();
        _dbContext.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}