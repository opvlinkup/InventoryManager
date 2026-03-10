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
    IHttpContextAccessor httpContextAccessor)
    : ITokenService
{
    public async Task<AuthTokensDto> GenerateAuthTokensAsync(User user, CancellationToken ct)
    {
       
            var roles = await users.GetRolesAsync(user);
            
            var (refresh, expires) = await jwt.GenerateRefreshTokenAsync(user, ct);

            var httpContext = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext unavailable");

            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshTokenHash = hash.ComputeHash(refresh),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expires,
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                DeviceFingerprint = httpContext.Request.Headers["X-Device-Fingerprint"].ToString()
            };
            
            var access = await jwt.GenerateAccessTokenAsync(user, roles, session.Id,  ct);

            await unitOfWork.SessionRepository.AddAsync(session, ct);

            await unitOfWork.CommitTransactionAsync(ct);

            return new AuthTokensDto
            {
                AccessToken = access,
                RefreshToken = refresh,
                RefreshTokenExpiresAt = expires
            };
    }

    public async Task<AuthTokensDto> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
            var refreshHash = hash.ComputeHash(refreshToken);

            var session = await unitOfWork.SessionRepository.GetByAsync(
                s => s.RefreshTokenHash == refreshHash,
                ct)
                ?? throw new UnauthorizedAccessException("Invalid refresh token");

            if (session.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Expired refresh token");

            var ctx = httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext unavailable");

            var fingerprint = ctx.Request.Headers["X-Device-Fingerprint"].ToString();

            if (session.DeviceFingerprint != fingerprint)
                throw new UnauthorizedAccessException("Device mismatch");

            if (session.IsRevoked)
            {
                var allSessions = await unitOfWork.SessionRepository.GetManyByAsync(
                    s => s.UserId == session.UserId && !s.IsRevoked,
                    null, null, true, ct);

                foreach (var s in allSessions)
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
            var access = await jwt.GenerateAccessTokenAsync(user, roles, session.Id,  ct);
            var (newRefresh, newExpires) = await jwt.GenerateRefreshTokenAsync(user, ct);

            var newSession = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RefreshTokenHash = hash.ComputeHash(newRefresh),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = newExpires,
                IpAddress = ctx.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx.Request.Headers["User-Agent"].ToString(),
                DeviceFingerprint = fingerprint
            };

            session.ReplacedByTokenHash = newSession.RefreshTokenHash;

            await unitOfWork.SessionRepository.AddAsync(newSession, ct);

            await unitOfWork.CommitTransactionAsync(ct);

            return new AuthTokensDto
            {
                AccessToken = access,
                RefreshToken = newRefresh,
                RefreshTokenExpiresAt = newExpires
            };
    }
}