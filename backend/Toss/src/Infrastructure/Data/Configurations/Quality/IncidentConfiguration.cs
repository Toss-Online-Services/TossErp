using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.Property(i => i.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(2000);

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.Type)
            .HasConversion<int>();

        builder.Property(i => i.Severity)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(i => i.Business)
            .WithMany()
            .HasForeignKey(i => i.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.QualityChecklist)
            .WithMany()
            .HasForeignKey(i => i.QualityChecklistId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.ChecklistItem)
            .WithMany()
            .HasForeignKey(i => i.ChecklistItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.ActionItems)
            .WithOne(a => a.Incident)
            .HasForeignKey(a => a.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => i.BusinessId);
        builder.HasIndex(i => i.Type);
        builder.HasIndex(i => i.Severity);
        builder.HasIndex(i => i.OccurredAt);
        builder.HasIndex(i => new { i.BusinessId, i.Severity, i.OccurredAt });
    }
}

