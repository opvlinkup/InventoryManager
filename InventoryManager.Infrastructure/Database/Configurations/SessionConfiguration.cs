using InventoryManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Property(x => x.RefreshTokenHash).IsRequired().HasColumnType("bytea");
        builder.Property(x => x.ReplacedByTokenHash).HasColumnType("bytea");



        builder.Property(x => x.IpAddress).HasMaxLength(100).IsUnicode(false);
        builder.Property(x => x.UserAgent).HasMaxLength(250);
        builder.Property(x => x.DeviceFingerprint).HasMaxLength(200);
        

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUsedAt);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.RevokedAt);
        
        

        builder.Property(x => x.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(100)
            .IsUnicode(false);


        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => x.IsRevoked);
        builder.HasIndex(x => x.RefreshTokenHash).IsUnique();
        builder.HasIndex(x => x.ReplacedByTokenHash);
    }
}