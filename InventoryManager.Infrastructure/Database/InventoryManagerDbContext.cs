using InventoryManager.Domain.Models;
using InventoryManager.Infrastructure.Email;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Infrastructure.Database;

public class InventoryManagerDbContext : IdentityUserContext<User, Guid>, IDataProtectionKeyContext{
    public InventoryManagerDbContext(DbContextOptions<InventoryManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } 
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<InventoryTag> InventoryTags { get; set; }
    public DbSet<CustomIdPart> CustomIdParts { get; set; }
    public DbSet<FieldMetadata> FieldDefinitions { get; set; }
    public DbSet<InventoryWriteAccess> InventoryWriteAccesses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<ItemCounter> InventorySequences { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(InventoryManagerDbContext).Assembly);
    }
}