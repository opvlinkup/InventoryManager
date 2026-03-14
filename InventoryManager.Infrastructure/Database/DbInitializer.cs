using InventoryManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryManager.Infrastructure.Database;


public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        try
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var db = serviceProvider.GetRequiredService<InventoryManagerDbContext>();

    
            var roles = new[] { "User", "Admin" };
        
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }
        
            var adminEmail = configuration["ADMIN_EMAIL"] ?? throw new InvalidOperationException("ADMIN_EMAIL is required");
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
                var result = await userManager.CreateAsync(admin, configuration["ADMIN_PASSWORD"] ?? throw new InvalidOperationException("ADMIN_PASSWORD is required"));
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            
            var predefinedCategories = new[]
            {
                new Category { Id = Guid.NewGuid(), Name = "Laptops" },
                new Category { Id = Guid.NewGuid(), Name = "Desktops" },
                new Category { Id = Guid.NewGuid(), Name = "Tablets" },
                new Category { Id = Guid.NewGuid(), Name = "Books" },
                new Category { Id = Guid.NewGuid(), Name = "Smartphones" }
            };

            foreach (var category in predefinedCategories)
            {
                if (!await db.Categories.AnyAsync(c => c.Name == category.Name))
                {
                   await db.Categories.AddAsync(category);
                }
            }
            await db.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
            logger.LogError(ex, "Error seeding roles and admin");
        }
    }
}