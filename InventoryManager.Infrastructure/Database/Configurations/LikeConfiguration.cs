using InventoryManager.Domain.Models;

namespace InventoryManager.Infrastructure.Database.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");
        
        builder.HasKey(x => new { x.UserId, x.ItemId });

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany(i => i.Likes)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.ItemId);
    }
}