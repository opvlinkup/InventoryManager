using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Jwt;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
}