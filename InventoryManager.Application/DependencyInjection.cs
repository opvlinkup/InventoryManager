using InventoryManager.Application.Abstractions.Auth;
using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Fields;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.Abstractions.Inventory.Likes;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.Abstractions.Session;
using InventoryManager.Application.Abstractions.Validators;
using InventoryManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using ILikeService = InventoryManager.Application.Abstractions.Inventory.Likes.ILikeService;

namespace InventoryManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IInventoryManagementService, InventoryManagementService>();
        services.AddScoped<IInventoryQueryService, InventoryQueryService>();
        services.AddScoped<IInventoryTemplateService, InventoryTemplateService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IInventoryAccessService, InventoryAccessService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICustomIdGenerator, CustomIdGenerator>();
        services.AddScoped<IFieldMetadataService,FieldMetadataService>();
        services.AddScoped<ICustomIdValidator, CustomIdValidator>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISessionService, SessionService>();
        return services;
    }
}