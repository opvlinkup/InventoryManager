using Google.Apis.Auth;
using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.DTO.Auth;
using Microsoft.Extensions.Configuration;

namespace InventoryManager.Application.Services;

public sealed class GoogleAuthService(IConfiguration configuration) : IGoogleAuthService
{
    public async Task<GoogleUserInfoDto> ValidateTokenAsync(string idToken, CancellationToken ct)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                    configuration["GoogleAuth:ClientId"]
                }
            });

        return new GoogleUserInfoDto
        {
            Email = payload.Email,
            Name = payload.Name,
            Picture = payload.Picture,
            GoogleId = payload.Subject
        };
    }
}