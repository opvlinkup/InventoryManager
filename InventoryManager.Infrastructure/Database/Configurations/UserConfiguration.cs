using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Surname)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Status)
            .HasConversion<int>();

        builder.Property(u => u.RegisteredAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        
        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique();
        
        builder.HasIndex(u => u.Status);
    }
}