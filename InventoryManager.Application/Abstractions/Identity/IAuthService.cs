using InventoryManager.Application.DTO;
using InventoryManager.Application.DTO.Auth;

namespace InventoryManager.Application.Abstractions.Identity;

public interface IAuthService
{
    public Task RegisterAsync(UserRegisterDto dto, CancellationToken ct);
    public Task<AuthTokensDto> LoginAsync(UserLoginDto dto, CancellationToken ct);
    public Task LogOutAsync(string refreshToken, CancellationToken ct);
}