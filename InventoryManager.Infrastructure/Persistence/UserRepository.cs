using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;

namespace InventoryManager.Infrastructure.Persistence;

public class UserRepository(InventoryManagerDbContext context) : Repository<User>(context), IUserRepository
{
    
}