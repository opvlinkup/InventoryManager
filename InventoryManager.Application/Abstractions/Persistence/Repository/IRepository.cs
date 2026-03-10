using System.Linq.Expressions;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<List<T>> GetManyAsync(int skip, int take, Func<IQueryable<T>, IQueryable<T>>? include, bool isTrackingEnabled, CancellationToken cancellationToken = default);
    
    Task<List<T>> GetManyByAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool isTrackingEnabled = false,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task RemoveRangeAsync(Expression<Func<T,bool>> filter, int batchSize, CancellationToken cancellationToken = default);
}