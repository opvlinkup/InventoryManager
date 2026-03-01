using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class InventoryTagConfiguration : IEntityTypeConfiguration<InventoryTag>
{
    public void Configure(EntityTypeBuilder<InventoryTag> builder)
    {
        builder.ToTable("InventoryTags");

        builder.HasKey(x => new { x.InventoryId, x.TagId });

        builder.HasOne(x => x.Inventory)
            .WithMany(i => i.Tags)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.Inventories)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TagId);
    }
}