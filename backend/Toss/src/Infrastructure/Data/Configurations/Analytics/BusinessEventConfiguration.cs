using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Analytics;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Analytics;

public class BusinessEventConfiguration : IEntityTypeConfiguration<BusinessEvent>
{
    public void Configure(EntityTypeBuilder<BusinessEvent> builder)
    {
        builder.Property(e => e.EventData)
            .HasMaxLength(2000);

        builder.Property(e => e.Module)
            .HasMaxLength(50);

        builder.Property(e => e.UserId)
            .HasMaxLength(450);

        builder.Property(e => e.EventType)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(e => e.Business)
            .WithMany()
            .HasForeignKey(e => e.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for efficient querying
        builder.HasIndex(e => e.BusinessId);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.OccurredAt);
        builder.HasIndex(e => new { e.BusinessId, e.EventType, e.OccurredAt });
        builder.HasIndex(e => new { e.BusinessId, e.Module, e.OccurredAt });
    }
}

