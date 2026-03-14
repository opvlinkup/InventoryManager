﻿using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();


        builder.HasOne(x => x.Inventory)
            .WithMany(i => i.Items)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.CreatedBy)
            .WithMany(u => u.CreatedItems)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CustomId).IsRequired().HasMaxLength(200);
        
        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnName("xmin");
        
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.Sequence);
        

        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_Item_CustomId_NotEmpty",
                "\"CustomId\" <> ''"));

        builder.HasIndex(x => new { x.InventoryId, x.CustomId })
            .IsUnique()
            .HasDatabaseName("UX_Item_Inventory_CustomId");
        
        builder.HasIndex(x => new { x.InventoryId, x.CreatedAt })
            .HasDatabaseName("IX_Item_Inventory_CreatedAt");
        
        builder.HasIndex(x => new { x.InventoryId, x.Sequence });
        
        builder.HasIndex(x => x.InventoryId);
        builder.HasIndex(x => x.CreatedById);
        builder.HasIndex(x => x.CreatedAt);
    }
}