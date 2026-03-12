using System.Text.RegularExpressions;
using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Messaging;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.DTO;
using InventoryManager.Application.DTO.Auth;
using InventoryManager.Application.Events;
using InventoryManager.Domain.Exceptions;
using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace InventoryManager.Application.Services;

public sealed class AuthService(
    UserManager<User> userManager,
    IHashingService hashingService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IConfiguration configuration,
    ILogger<AuthService> logger,
    IHttpContextAccessor httpAccessor,
    IGoogleAuthService googleAuthService,
    ICurrentUserService currentUserService)
    : IAuthService
{
    public async Task RegisterAsync(UserRegisterDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email and password are required");

        if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ValidationException("Email is not valid");
        
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Surname = dto.Surname,
            RegisteredAt = DateTime.UtcNow,
            Status = Status.Unverified,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = false,
            Language = Language.En,
            Theme = Theme.Dark
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                    e.Code == nameof(IdentityErrorDescriber.DuplicateEmail) ||
                    e.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
            {
                logger.LogWarning("Email is already taken: {Email}", dto.Email);
                throw new EmailException(dto.Email);
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException(errors);
        }
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        var integrationEvent = new UserRegisteredIntegrationEvent()
        {
            UserId = user.Id,
            Email = user.Email,
            ConfirmationToken = encodedToken
        };
        
        await publisher.PublishAsync(integrationEvent, ct);

    }

    public async Task<AuthTokensDto> LoginAsync(UserLoginDto dto, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");
        
        if (!await userManager.CheckPasswordAsync(user, dto.Password) || user.Status == Status.Blocked)
            throw new UnauthorizedAccessException("Invalid credentials");
        
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        await userManager.UpdateAsync(user);
        
        return await tokenService.GenerateAuthTokensAsync(user, ct);
    }
    
    public async Task<AuthTokensDto> LoginWithGoogleAsync(string idToken, CancellationToken ct)
    {
        var googleUser = await googleAuthService
            .ValidateTokenAsync(idToken, ct);

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleUser.GoogleId, ct);

        if (user == null)
        {
            user = new User
            {
                Email = googleUser.Email,
                UserName = googleUser.Email,
                Name = googleUser.Name,
                GoogleId = googleUser.GoogleId,
                EmailConfirmed = true,
                Status = Status.Active,
                RegisteredAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
                throw new ApplicationException("Failed to create user");
        }

        return await tokenService.GenerateAuthTokensAsync(user, ct);
    }


    public async Task LogOutAsync(string refreshToken, CancellationToken ct)
    {
        var hash = hashingService.ComputeHash(refreshToken);

        var session = await unitOfWork.SessionRepository.GetByAsync(
            s => s.RefreshTokenHash == hash, ct);
        
        if (session == null || session.IsRevoked)
            return;
        
        session.IsRevoked = true;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedByIp = currentUserService.IpAddress;
    }
    
    public async Task ConfirmEmailAsync(Guid userId, string token, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException("Invalid confirmation link");

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid or expired confirmation token");

        if (user.Status != Status.Blocked)
        {
            user.Status = Status.Active;
            await userManager.UpdateAsync(user);
        }
    }
    
}