namespace InventoryManager.Infrastructure.Jwt;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SigningKey { get; init; } = null!;
    public int AccessTokenLifetimeMinutes { get; init; }
    public int RefreshTokenLifetimeDays { get; init; }
}