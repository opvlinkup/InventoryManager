using System.Linq.Expressions;
using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class Repository<T>(InventoryManagerDbContext context) : IRepository<T>
    where T : class
{
    
    private readonly InventoryManagerDbContext _dbContext =
        context ?? throw new ArgumentNullException(nameof(context));
    
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
      
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be an empty GUID.", nameof(id));
        }
        
        return await _dbSet.FindAsync([id], cancellationToken);
    }
    
    public async Task<List<T>> GetManyAsync(
        int skip,
        int take,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool isTrackingEnabled = true,
        CancellationToken cancellationToken = default
    )
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), "Skip must be a non-negative number.");
        }
        
        if (take <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(take), "Take must be a positive number.");
        }
        
        IQueryable<T> query = _dbSet;

        if (include is not null)
        {query = include(query);}
        
        if (isTrackingEnabled)
        {
            return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        return await query.AsNoTracking().Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetManyByAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool isTrackingEnabled = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        IQueryable<T> query = _dbSet.Where(filter);

        if (include is not null)
            query = include(query);

        if (orderBy is not null)
            query = orderBy(query);

        if (isTrackingEnabled)
        {
            return await query.ToListAsync(cancellationToken);
        }

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _dbSet.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _dbSet.AnyAsync(filter, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {

        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbSet.RemoveRange(entities);
    }
    
    public async Task RemoveRangeAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        if (batchSize > 2000) batchSize = 2000;

        while (true)
        {
            var deletedCount = await _dbSet
                .Take(batchSize)
                .ExecuteDeleteAsync(cancellationToken);
            
            if (deletedCount < batchSize)
                break;
        }
    }
    
    
}