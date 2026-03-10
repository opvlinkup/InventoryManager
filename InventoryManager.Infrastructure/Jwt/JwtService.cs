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

    public Task<string> GenerateAccessTokenAsync(
        User user,
        IEnumerable<string> roles,
        Guid sessionId,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),

            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new Claim(
                JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),

            new Claim(JwtRegisteredClaimNames.Sid, sessionId.ToString()),

            new Claim("status", user.Status.ToString())
        };

        foreach (var role in roles)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenLifetimeMinutes),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512)
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return Task.FromResult(tokenHandler.WriteToken(token));
    }
    

    public Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(User user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var randomBytes = RandomNumberGenerator.GetBytes(64);

        var token = WebEncoders.Base64UrlEncode(randomBytes);

        var expiration = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays);

        return Task.FromResult((token, expiration));
    }

    private static void ValidateOptions(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SigningKey))
            throw new InvalidOperationException("JWT signing key is missing");

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < 64)
            throw new InvalidOperationException(
                "JWT signing key must be at least 64 bytes for HS512");

        if (options.AccessTokenLifetimeMinutes <= 0 || options.AccessTokenLifetimeMinutes > 10)
            throw new InvalidOperationException(
                "Access token lifetime is invalid");

        if (options.RefreshTokenLifetimeDays <= 0 || options.RefreshTokenLifetimeDays > 30)
            throw new InvalidOperationException(
                "Refresh token lifetime is invalid");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            throw new InvalidOperationException("JWT issuer is missing");

        if (string.IsNullOrWhiteSpace(options.Audience))
            throw new InvalidOperationException("JWT audience is missing");
    }
}