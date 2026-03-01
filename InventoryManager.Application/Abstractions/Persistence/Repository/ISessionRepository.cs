using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Persistence;

public interface ISessionRepository : IRepository<Session>
{
    Task<Session?> GetByTokenHashAsync(byte[] hash, CancellationToken ct);
}