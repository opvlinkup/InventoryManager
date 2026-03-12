using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventoryManager.Infrastructure.Database;

public sealed class InventoryManagerDbContextFactory : IDesignTimeDbContextFactory<InventoryManagerDbContext>
{
    public InventoryManagerDbContext CreateDbContext(string[] args)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../InventoryManager.API");
        
        if (!Directory.Exists(path))
            path = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(path, "appsettings.json"), optional: false)
            .AddEnvironmentVariables()
            .Build();

        // Try to get connection string from DB_CONNECTION env variable first, then from appsettings
        var connectionString = configuration["DB_CONNECTION_LOCAL"] 
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found. Set DB_CONNECTION environment variable or DefaultConnection in appsettings.json");

        var optionsBuilder = new DbContextOptionsBuilder<InventoryManagerDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new InventoryManagerDbContext(optionsBuilder.Options);
    }
}