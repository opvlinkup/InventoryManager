using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Jwt;

public interface IJwtService
{
    public Task<string> GenerateAccessTokenAsync(User user, IEnumerable<string> roles, Guid sessionId, CancellationToken ct = default);
    Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
}