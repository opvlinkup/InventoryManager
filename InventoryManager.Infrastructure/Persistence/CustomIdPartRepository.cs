using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;

namespace InventoryManager.Infrastructure.Persistence;

public class CustomIdPartRepository(InventoryManagerDbContext context) :Repository<CustomIdPart>(context),ICustomIdPartRepository
{
}