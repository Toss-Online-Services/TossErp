using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Aggregates;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ProjectIssue aggregate
/// </summary>
public class ProjectIssueConfiguration : IEntityTypeConfiguration<ProjectIssue>
{
    public void Configure(EntityTypeBuilder<ProjectIssue> builder)
    {
        // Table configuration
        builder.ToTable("ProjectIssues", "projects");
        
        // Primary key
        builder.HasKey(i => i.Id);
        
        // Properties
        builder.Property(i => i.Id)
            .IsRequired();

        builder.Property(i => i.ProjectId)
            .IsRequired();

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.AssigneeId);

        builder.Property(i => i.AssigneeName)
            .HasMaxLength(255);

        builder.Property(i => i.Resolution)
            .HasMaxLength(2000);

        builder.Property(i => i.ReportedDate)
            .IsRequired();

        builder.Property(i => i.TargetResolutionDate);
        builder.Property(i => i.ActualResolutionDate);

        builder.Property(i => i.ReportedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(i => i.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(i => i.Priority, p =>
        {
            p.Property(x => x.Level)
                .HasColumnName("IssuePriority")
                .IsRequired()
                .HasMaxLength(50);
        });

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(i => i.ProjectId)
            .HasDatabaseName("IX_ProjectIssues_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(i => new { i.ProjectId, i.Status })
            .HasDatabaseName("IX_ProjectIssues_ProjectId_Status")
            .HasFillFactor(90);

        builder.HasIndex(i => new { i.ProjectId, i.AssigneeId })
            .HasDatabaseName("IX_ProjectIssues_ProjectId_AssigneeId")
            .HasFillFactor(90);

        builder.HasIndex(i => new { i.ProjectId, i.Severity })
            .HasDatabaseName("IX_ProjectIssues_ProjectId_Severity")
            .HasFillFactor(90);

        builder.HasIndex(i => i.ReportedDate)
            .HasDatabaseName("IX_ProjectIssues_ReportedDate")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(i => i.DomainEvents);
    }
}
