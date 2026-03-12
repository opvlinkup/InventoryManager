using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class FieldMetadataConfiguration : IEntityTypeConfiguration<FieldMetadata>
{
    public void Configure(EntityTypeBuilder<FieldMetadata> builder)
    {
        builder.ToTable("FieldMetadata");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.Inventory)
            .WithMany(i => i.FieldDefinitions)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.InventoryId)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.State)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Slot)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Tooltip)
            .HasMaxLength(400);

        builder.Property(x => x.ShowInUiTable)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();
        
        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_FieldMetadata_Slot_Range",
                "\"Slot\" >= 1 AND \"Slot\" <= 3"));
        builder.HasIndex(x => new { x.InventoryId, x.Type, x.Slot })
            .IsUnique()
            .HasDatabaseName("UX_FieldMetadata_Inventory_Type_Slot");
        
        builder.HasIndex(x => new { x.InventoryId, x.Order })
            .IsUnique()
            .HasDatabaseName("UX_FieldMetadata_Inventory_Order");

        builder.HasIndex(x => x.InventoryId);
    }
}