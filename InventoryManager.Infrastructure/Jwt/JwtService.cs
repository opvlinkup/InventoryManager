using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InventoryManager.Application.Abstractions.Jwt;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace InventoryManager.Infrastructure.Jwt;

public sealed class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IOptions<JwtOptions> options, ILogger<JwtService> logger)
    {
        _options = options.Value;
        _logger = logger;

        ValidateOptions(_options);

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
    }

    public Task<string> GenerateAccessTokenAsync(User user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("status", user.Status.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenLifetimeMinutes),
            signingCredentials: new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha512)
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(
        User user,
        CancellationToken ct = default)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = WebEncoders.Base64UrlEncode(randomBytes);

        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays);

        return Task.FromResult((token, expiresAt));
    }

    private static void ValidateOptions(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SigningKey))
            throw new InvalidOperationException("JWT signing key is missing");

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < 64)
            throw new InvalidOperationException(
                "JWT signing key must be at least 64 bytes for HS512");

        if (options.AccessTokenLifetimeMinutes <= 0)
            throw new InvalidOperationException("Access token lifetime is invalid");

        if (options.RefreshTokenLifetimeDays <= 0)
            throw new InvalidOperationException("Refresh token lifetime is invalid");
    }
}