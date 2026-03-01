using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(i=> i.Comments)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_Comments_ItemId");

        builder.HasIndex(x => new { x.ItemId, x.CreatedAt })
            .HasDatabaseName("IX_Comments_ItemId_CreatedAt");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Comments_UserId");
    }
}