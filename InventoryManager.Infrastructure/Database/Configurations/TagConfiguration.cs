using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnType("citext")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("UX_Tag_Name");
    }
}