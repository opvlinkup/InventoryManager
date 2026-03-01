namespace InventoryManager.Infrastructure.Database.Configurations;

using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomIdPartConfiguration : IEntityTypeConfiguration<CustomIdPart>
{
    public void Configure(EntityTypeBuilder<CustomIdPart> builder)
    {
        builder.ToTable("CustomIdParts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();


        builder.HasOne(x => x.Inventory)
            .WithMany(i => i.CustomIdParts)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.InventoryId)
            .IsRequired();
        
        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.Format)
            .HasMaxLength(200);

        builder.Property(x => x.FixedValue)
            .HasMaxLength(200);


        builder.HasIndex(x => new { x.InventoryId, x.Order }).IsUnique().HasDatabaseName("UX_CustomIdPart_Inventory_Order");
        
        builder.HasIndex(x => x.InventoryId);
    }
}