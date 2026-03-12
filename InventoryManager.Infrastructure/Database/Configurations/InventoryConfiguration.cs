using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");


        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();


        builder.HasOne(x => x.Owner)
            .WithMany(u => u.OwnedInventories)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.OwnerId)
            .IsRequired();


        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(x => x.IsPublic)
            .HasDefaultValue(false);


        builder.HasOne(x => x.Category)
            .WithMany(c => c.Inventories)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CategoryId)
            .IsRequired();


        builder.Property(x => x.WriteAccessMode)
            .IsRequired()
            .HasConversion<int>();


        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();


        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();



        builder.HasMany(x => x.Items)
            .WithOne(i => i.Inventory)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.InventoryWriteAccesses)
            .WithOne(a => a.Inventory)
            .HasForeignKey(a => a.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FieldDefinitions)
            .WithOne(f => f.Inventory)
            .HasForeignKey(f => f.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CustomIdParts)
            .WithOne(p => p.Inventory)
            .HasForeignKey(p => p.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
        

        builder.HasMany(x => x.Tags)
            .WithOne(t => t.Inventory)
            .HasForeignKey(t => t.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.IsPublic);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.UpdatedAt);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => new { x.OwnerId, x.Title }).IsUnique();
    }
}