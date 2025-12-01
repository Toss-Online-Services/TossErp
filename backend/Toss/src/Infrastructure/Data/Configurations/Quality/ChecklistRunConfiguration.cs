using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Quality;

namespace Toss.Infrastructure.Data.Configurations.Quality;

public class ChecklistRunConfiguration : IEntityTypeConfiguration<ChecklistRun>
{
    public void Configure(EntityTypeBuilder<ChecklistRun> builder)
    {
        builder.Property(r => r.RunByUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(r => r.Business)
            .WithMany()
            .HasForeignKey(r => r.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.QualityChecklist)
            .WithMany(c => c.Runs)
            .HasForeignKey(r => r.QualityChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Results)
            .WithOne(ri => ri.ChecklistRun)
            .HasForeignKey(ri => ri.ChecklistRunId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.BusinessId);
        builder.HasIndex(r => r.QualityChecklistId);
        builder.HasIndex(r => r.RunDate);
        builder.HasIndex(r => r.RunByUserId);
    }
}

