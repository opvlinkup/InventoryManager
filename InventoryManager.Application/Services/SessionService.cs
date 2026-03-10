using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Session;

namespace InventoryManager.Application.Services;

public sealed class SessionService(IUnitOfWork unitOfWork) : ISessionService
{
    public async Task UpdateLastActivityAsync(Guid userId, CancellationToken ct)
    {
        var session = await unitOfWork.SessionRepository
            .GetByAsync(s => s.UserId == userId, ct);

        if (session == null)
            return;

        if (session.LastUsedAt > DateTime.UtcNow.AddMinutes(-5))
            return;

        session.LastUsedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(ct);
    }
}