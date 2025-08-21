using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ProjectMilestone entity
/// </summary>
public class ProjectMilestoneConfiguration : IEntityTypeConfiguration<ProjectMilestone>
{
    public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
    {
        // Table configuration
        builder.ToTable("ProjectMilestones", "projects");
        
        // Primary key
        builder.HasKey(m => m.Id);
        
        // Properties
        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(m => m.ProjectId)
            .IsRequired();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(m => m.PlannedDate)
            .IsRequired();

        builder.Property(m => m.ActualDate);

        builder.Property(m => m.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(m => m.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(m => m.Progress, p =>
        {
            p.Property(x => x.CompletedItems)
                .HasColumnName("MilestoneProgressCompleted")
                .IsRequired();
            
            p.Property(x => x.TotalItems)
                .HasColumnName("MilestoneProgressTotal")
                .IsRequired();
        });

        // Relationships
        builder.HasMany(m => m.Tasks)
            .WithOne()
            .HasForeignKey(t => t.MilestoneId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(m => m.ProjectId)
            .HasDatabaseName("IX_ProjectMilestones_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(m => new { m.ProjectId, m.Status })
            .HasDatabaseName("IX_ProjectMilestones_ProjectId_Status")
            .HasFillFactor(90);

        builder.HasIndex(m => new { m.ProjectId, m.PlannedDate })
            .HasDatabaseName("IX_ProjectMilestones_ProjectId_PlannedDate")
            .HasFillFactor(90);

        builder.HasIndex(m => m.PlannedDate)
            .HasDatabaseName("IX_ProjectMilestones_PlannedDate")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(m => m.DomainEvents);
    }
}
