using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Jwt;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO.Auth;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace InventoryManager.Infrastructure.Jwt;

public sealed class TokenService(
    IJwtService jwt,
    IUnitOfWork unitOfWork,
    UserManager<User> users,
    IHashingService hash,
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserService currentUserService)
    : ITokenService
{
    public async Task<AuthTokensDto> GenerateAuthTokensAsync(User user, CancellationToken ct)
    {
        var roles = await users.GetRolesAsync(user);

        var (refreshToken, expires) = await jwt.GenerateRefreshTokenAsync(user, ct);

        var refreshHash = hash.ComputeHash(refreshToken);

        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = refreshHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expires,
            IpAddress = currentUserService.IpAddress,
            UserAgent = currentUserService.UserAgent,
            DeviceFingerprint = currentUserService.DeviceFingerprint
        };

        var access = await jwt.GenerateAccessTokenAsync(user, roles, session.Id, ct);

        await unitOfWork.SessionRepository.AddAsync(session, ct);

        await unitOfWork.CommitTransactionAsync(ct);

        return new AuthTokensDto
        {
            AccessToken = access,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = expires
        };
    }
public async Task<AuthTokensDto> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(refreshToken))
        throw new UnauthorizedAccessException("Invalid refresh token");

    var refreshHash = hash.ComputeHash(refreshToken);

    var session = await unitOfWork.SessionRepository.GetByAsync(
        s => s.RefreshTokenHash.SequenceEqual(refreshHash),
        ct) ?? throw new UnauthorizedAccessException("Invalid refresh token");

    if (session.ExpiresAt <= DateTime.UtcNow)
        throw new UnauthorizedAccessException("Expired refresh token");
    

    if (session.IsRevoked)
    {
        var sessions = await unitOfWork.SessionRepository.GetManyByAsync(
            s => s.UserId == session.UserId && !s.IsRevoked,
            null, null, true, ct);

        foreach (var s in sessions)
        {
            s.IsRevoked = true;
            s.RevokedAt = DateTime.UtcNow;
        }

        await unitOfWork.CommitTransactionAsync(ct);

        throw new UnauthorizedAccessException("Token reuse detected");
    }

    session.IsRevoked = true;
    session.RevokedAt = DateTime.UtcNow;

    var user = await users.FindByIdAsync(session.UserId.ToString())
        ?? throw new UnauthorizedAccessException();

    var roles = await users.GetRolesAsync(user);

    var access = await jwt.GenerateAccessTokenAsync(user, roles, session.Id, ct);

    var (newRefreshToken, newExpires) = await jwt.GenerateRefreshTokenAsync(user, ct);

    var newHash = hash.ComputeHash(newRefreshToken);

    var newSession = new Session
    {
        Id = Guid.NewGuid(),
        UserId = user.Id,
        RefreshTokenHash = newHash,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = newExpires,
        IpAddress = currentUserService.IpAddress,
        UserAgent = currentUserService.UserAgent,
        DeviceFingerprint = currentUserService.DeviceFingerprint
    };

    session.ReplacedByTokenHash = newHash;

    await unitOfWork.SessionRepository.AddAsync(newSession, ct);

    await unitOfWork.CommitTransactionAsync(ct);

    return new AuthTokensDto
    {
        AccessToken = access,
        RefreshToken = newRefreshToken,
        RefreshTokenExpiresAt = newExpires
    };
}
}