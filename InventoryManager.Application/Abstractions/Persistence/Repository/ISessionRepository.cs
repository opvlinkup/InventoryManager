using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence.Repository;

public interface ISessionRepository : IRepository<Domain.Models.Session>
{
    Task<Domain.Models.Session?> GetByTokenHashAsync(byte[] hash, CancellationToken ct);
}