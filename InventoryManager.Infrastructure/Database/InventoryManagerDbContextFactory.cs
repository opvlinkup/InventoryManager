using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryManager.Infrastructure.Database;

public sealed class InventoryManagerDbContextFactory 
    : IDesignTimeDbContextFactory<InventoryManagerDbContext>
{
    public InventoryManagerDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(),"..");

        var envPath = Path.Combine(basePath, ".env");

        if (!File.Exists(envPath))
            throw new InvalidOperationException($".env file not found at: {envPath}");

        Env.Load(envPath);

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_LOCAL");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Environment variable 'DB_CONNECTION_LOCAL' is not set or empty."
            );

        var optionsBuilder = new DbContextOptionsBuilder<InventoryManagerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new InventoryManagerDbContext(optionsBuilder.Options);
    }
}