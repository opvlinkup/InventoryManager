using System.Linq.Expressions;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface IUserRepository : IRepository<User>
{
}