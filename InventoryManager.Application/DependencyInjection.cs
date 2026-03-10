using InventoryManager.Application.Abstractions.Identity;
using InventoryManager.Application.Abstractions.Inventory;
using InventoryManager.Application.Abstractions.Inventory.Items;
using InventoryManager.Application.Abstractions.Persistence.UnitOfWork;
using InventoryManager.Application.Abstractions.Security;
using InventoryManager.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace InventoryManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IInventoryManagementService, IInventoryManagementService>();
        services.AddScoped<IInventoryQueryService, InventoryQueryService>();
        services.AddScoped<IInventoryTemplateService, InventoryTemplateService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IInventoryAccessService, InventoryAccessService>();
        services.AddScoped<IUnitOfWork, IUnitOfWork>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}