using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Persistence;

public class SessionRepository(InventoryManagerDbContext context) : Repository<Session>(context), ISessionRepository
{
    public async Task<Session?> GetByTokenHashAsync(byte[] hash, CancellationToken ct)
    {
        if (hash.Length == 0)
            return null;
        
        return await context.Sessions
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == hash, ct);
    }
}