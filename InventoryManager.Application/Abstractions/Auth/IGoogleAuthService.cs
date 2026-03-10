using InventoryManager.Application.DTO.Auth;

namespace InventoryManager.Application.Abstractions.Auth;

public interface IGoogleAuthService
{
    Task<GoogleUserInfoDto> ValidateTokenAsync(string idToken, CancellationToken ct);
}