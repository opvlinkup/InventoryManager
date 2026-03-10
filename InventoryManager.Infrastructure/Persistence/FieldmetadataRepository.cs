using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;

namespace InventoryManager.Infrastructure.Persistence;

public class FieldmetadataRepository(InventoryManagerDbContext context) : Repository<FieldMetadata>(context),IFieldMetadataRepository
{
    
}