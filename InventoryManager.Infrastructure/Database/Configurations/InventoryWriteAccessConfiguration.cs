using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;


public class InventoryWriteAccessConfiguration : IEntityTypeConfiguration<InventoryWriteAccess>
{
    public void Configure(EntityTypeBuilder<InventoryWriteAccess> builder)
    {
        builder.ToTable("InventoryWriteAccess");

        builder.HasKey(x => new { x.InventoryId, x.UserId });

        builder.HasOne(x => x.Inventory)
            .WithMany(i => i.InventoryWriteAccesses)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.InventoryWriteAccesses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
    }
}