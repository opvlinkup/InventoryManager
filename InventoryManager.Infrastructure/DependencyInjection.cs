using System.Text;
using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.Abstractions.Email;
using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Jwt;
using InventoryManager.Application.Abstractions.Messaging;
using InventoryManager.Application.Abstractions.Persistence;
using InventoryManager.Application.Abstractions.Persistence.Repository;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.Services;
using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Database;
using InventoryManager.Infrastructure.Email;
using InventoryManager.Infrastructure.Identity;
using InventoryManager.Infrastructure.Jwt;
using InventoryManager.Infrastructure.Messaging;
using InventoryManager.Infrastructure.Persistence;
using InventoryManager.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "keys")))
            .SetApplicationName("InventoryManager");

        services.AddDbContext<InventoryManagerDbContext>(options =>
            options.UseNpgsql(configuration["DB_CONNECTION_LOCAL"]));

        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 8;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<InventoryManagerDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddDataProtection()
            .PersistKeysToDbContext<InventoryManagerDbContext>()
            .SetApplicationName("InventoryManager");
        

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryWriteAccessRepository, InventoryWriteAccessRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomIdPartRepository, CustomIdPartRepository>();
        services.AddScoped<IFieldMetadataRepository, FieldmetadataRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();
        services.AddScoped<ICookieService, CookieService>();
        
        services.AddSignalR();
        services.AddHttpClient();
        services.AddScoped<IEmailService, EmailService>();


        return services;
    }
}