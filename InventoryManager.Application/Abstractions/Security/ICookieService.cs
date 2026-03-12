using InventoryManager.Application.DTO.Auth;
using Microsoft.AspNetCore.Http;

namespace InventoryManager.Application.Abstractions.Security;

public interface ICookieService
{
    void AppendAuthCookies(HttpResponse response, AuthTokensDto tokens);
    void DeleteAuthCookies(HttpResponse response);
    string GetRefreshToken(HttpRequest request);
}