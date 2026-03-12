using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class InventorySequenceConfiguration : IEntityTypeConfiguration<ItemCounter>
{
    public void Configure(EntityTypeBuilder<ItemCounter> builder)
    {
        builder.ToTable("InventorySequences");

        builder.HasKey(x => x.InventoryId);

        builder.Property(x => x.NextValue)
            .IsRequired();

        builder.HasOne(x => x.Inventory)
            .WithOne()
            .HasForeignKey<ItemCounter>(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}