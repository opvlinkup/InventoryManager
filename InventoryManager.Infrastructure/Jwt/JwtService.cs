using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InventoryManager.Application.Abstractions.Jwt;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace InventoryManager.Infrastructure.Jwt;

public sealed class JwtService : IJwtService
{
   
    private readonly SymmetricSecurityKey _signingKey;
    private readonly ILogger<JwtService> _logger;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _logger = logger;
        
        var signingKey = configuration["JWT_SEC_KEY"];
        _issuer = configuration["ISSUER"] ?? throw new InvalidOperationException("JWT issuer is missing");
        _audience = configuration["AUDIENCE"] ?? throw new InvalidOperationException("JWT audience is missing");
        _accessTokenMinutes = int.TryParse(configuration["JJWT_ACCESS_LIFETIME_MINUTES"], out var m) ? m : 5;
        _refreshTokenDays = int.TryParse(configuration["JWT_REFRESH_LIFETIME_DAYS"], out var d) ? d : 7;

        // Проверка ключа
        if (string.IsNullOrWhiteSpace(signingKey))
            throw new InvalidOperationException("JWT signing key is missing");

        if (Encoding.UTF8.GetByteCount(signingKey) < 64)
            throw new InvalidOperationException("JWT signing key must be at least 64 bytes for HS512");

        if (string.IsNullOrWhiteSpace(_issuer))
            throw new InvalidOperationException("JWT issuer is missing");

        if (string.IsNullOrWhiteSpace(_audience))
            throw new InvalidOperationException("JWT audience is missing");

        if (_accessTokenMinutes <= 0 || _accessTokenMinutes > 10)
            throw new InvalidOperationException("Access token lifetime is invalid");

        if (_refreshTokenDays <= 0 || _refreshTokenDays > 30)
            throw new InvalidOperationException("Refresh token lifetime is invalid");

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
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
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_accessTokenMinutes),
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

        var expiration = DateTime.UtcNow.AddDays(_refreshTokenDays);

        return Task.FromResult((token, expiration));
    }
    
}