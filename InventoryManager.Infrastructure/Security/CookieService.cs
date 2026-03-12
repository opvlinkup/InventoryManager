using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InventoryManager.Infrastructure.Security;

public sealed class CookieService(IConfiguration configuration) : ICookieService
{
    public void AppendAuthCookies(HttpResponse response, AuthTokensDto tokens)
    {
        var lifetimeMinutes = configuration.GetValue<int>("AuthOptions:Lifetime");

        response.Cookies.Append(
            "accessToken",
            tokens.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(lifetimeMinutes),
                Path = "/"
            });

        response.Cookies.Append(
            "refreshToken",
            tokens.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokens.RefreshTokenExpiresAt,
                Path = "/"
            });
    }

    public void DeleteAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete("accessToken", new CookieOptions { Path = "/" });
        response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
    }

    public string GetRefreshToken(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue("refreshToken", out var token))
            throw new UnauthorizedAccessException("Refresh token missing");

        return token;
    }
}