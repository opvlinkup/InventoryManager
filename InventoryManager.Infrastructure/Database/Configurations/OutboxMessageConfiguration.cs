using InventoryManager.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManager.Infrastructure.Database.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasMaxLength(256).IsRequired();
        builder.Property(m => m.Content).HasMaxLength(4000).IsRequired();
        builder.Property(m => m.OccurredOn).IsRequired();
        builder.Property(m => m.ProcessedOn).IsRequired(false);
        builder.Property(m => m.Error).HasMaxLength(4000).IsRequired(false);
        builder.Property(m => m.ConfirmationToken).HasMaxLength(4000).IsRequired(false);
        
        builder.HasIndex(m => m.ProcessedOn);
    }
}