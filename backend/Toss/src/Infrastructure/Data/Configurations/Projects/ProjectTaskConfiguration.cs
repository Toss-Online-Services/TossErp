using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Projects;

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Property(t => t.EstimatedHours)
            .HasPrecision(18, 2);

        builder.Property(t => t.ActualHours)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(t => t.Business)
            .WithMany()
            .HasForeignKey(t => t.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.BusinessId);
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.AssigneeId);
    }
}

