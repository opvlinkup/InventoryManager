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


        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<InventoryManagerDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new InventoryManagerDbContext(optionsBuilder.Options);
    }
}