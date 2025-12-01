using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Projects;

namespace Toss.Infrastructure.Data.Configurations.Projects;

public class LabourEntryConfiguration : IEntityTypeConfiguration<LabourEntry>
{
    public void Configure(EntityTypeBuilder<LabourEntry> builder)
    {
        builder.Property(l => l.Hours)
            .HasPrecision(18, 2);

        builder.Property(l => l.Rate)
            .HasPrecision(18, 2);

        builder.Property(l => l.TotalCost)
            .HasPrecision(18, 2);

        builder.Property(l => l.Description)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(l => l.Business)
            .WithMany()
            .HasForeignKey(l => l.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Project)
            .WithMany(p => p.LabourEntries)
            .HasForeignKey(l => l.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.ProjectTask)
            .WithMany()
            .HasForeignKey(l => l.ProjectTaskId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(l => l.BusinessId);
        builder.HasIndex(l => l.ProjectId);
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.WorkDate);
        builder.HasIndex(l => l.ProjectTaskId);
    }
}

