using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Collaborations;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Collaborations;

public class CollabLinkConfiguration : IEntityTypeConfiguration<CollabLink>
{
    public void Configure(EntityTypeBuilder<CollabLink> builder)
    {
        builder.Property(l => l.LinkCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.LinkedType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.Purpose)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(l => l.Business)
            .WithMany()
            .HasForeignKey(l => l.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(l => l.LinkCode)
            .IsUnique(); // Unique link code

        builder.HasIndex(l => l.BusinessId);
        builder.HasIndex(l => new { l.BusinessId, l.LinkedType, l.LinkedId, l.Purpose });
        builder.HasIndex(l => l.IsActive);
        builder.HasIndex(l => l.ExpiresAt);
    }
}

