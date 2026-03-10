namespace InventoryManager.Application.DTO.Auth;

public sealed class GoogleUserInfoDto
{
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Picture { get; init; } = null!;
    public string GoogleId { get; init; } = null!;
}