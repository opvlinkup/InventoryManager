using System.Linq.Expressions;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<List<User>> GetUsersOrderedAsync<TKey>(Expression<Func<User, TKey>> orderBy, bool descending,
        CancellationToken ct, int count = 50);
}