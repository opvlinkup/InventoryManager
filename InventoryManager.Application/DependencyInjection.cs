using Microsoft.Extensions.DependencyInjection;

namespace InventoryManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection));
        return services;
    }
}