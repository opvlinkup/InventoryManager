using System.Security.Cryptography;
using System.Text;
using InventoryManager.Application.Abstractions.Security;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace InventoryManager.Infrastructure.Security;

public sealed class HashingService : IHashingService
{
    private readonly byte[] _pepper;

    public HashingService(IConfiguration config)
    {
        var secret = config["REFRESH_TOKEN_PEPPER"];
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("Refresh token pepper missing");

        _pepper = Encoding.UTF8.GetBytes(secret);
    }

    public byte[] ComputeHash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = WebEncoders.Base64UrlDecode(token);

        using var hmac = new HMACSHA256(_pepper);
        return hmac.ComputeHash(tokenBytes);
    }
}