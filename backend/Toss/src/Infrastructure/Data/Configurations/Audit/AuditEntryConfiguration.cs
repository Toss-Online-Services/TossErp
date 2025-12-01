using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Audit;

namespace Toss.Infrastructure.Data.Configurations.Audit;

public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.Property(a => a.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.UserName)
            .HasMaxLength(256);

        builder.Property(a => a.Changes)
            .HasMaxLength(4000); // JSON diff

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        // Relationships
        builder.HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.BusinessId);
        builder.HasIndex(a => new { a.BusinessId, a.EntityType, a.EntityId });
        builder.HasIndex(a => new { a.BusinessId, a.Created });
        builder.HasIndex(a => a.UserId);
    }
}

