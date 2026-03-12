using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManager.Infrastructure.Database;


public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    
        var roles = new[] { "User", "Admin" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
            }
        }
        
        string adminEmail = "admin@gmail.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin",
                Surname = "Administrator",
                Status = Status.Active,
                EmailConfirmed = true,
                RegisteredAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "BestAdmin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}