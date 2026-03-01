using InventoryManager.Application.DTO.Auth;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Abstractions.Identity;

public interface ITokenService
{
    Task<AuthTokensDto> GenerateAuthTokensAsync(User user, CancellationToken ct);
    Task<AuthTokensDto> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct);
}