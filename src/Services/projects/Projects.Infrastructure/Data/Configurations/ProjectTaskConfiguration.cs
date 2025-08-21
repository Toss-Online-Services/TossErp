using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ProjectTask entity
/// </summary>
public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        // Table configuration
        builder.ToTable("ProjectTasks", "projects");
        
        // Primary key
        builder.HasKey(t => t.Id);
        
        // Properties
        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.Property(t => t.TaskNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.TaskType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.AssigneeId);

        builder.Property(t => t.AssigneeName)
            .HasMaxLength(255);

        builder.Property(t => t.ParentTaskId);
        builder.Property(t => t.MilestoneId);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(t => t.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(t => t.DateRange, dr =>
        {
            dr.Property(x => x.StartDate)
                .HasColumnName("TaskStartDate");
            
            dr.Property(x => x.EndDate)
                .HasColumnName("TaskEndDate");
        });

        builder.ComplexProperty(t => t.Effort, e =>
        {
            e.Property(x => x.EstimatedHours)
                .HasColumnName("EstimatedHours")
                .HasColumnType("decimal(8,2)");
            
            e.Property(x => x.ActualHours)
                .HasColumnName("ActualHours")
                .HasColumnType("decimal(8,2)");
        });

        builder.ComplexProperty(t => t.Progress, p =>
        {
            p.Property(x => x.CompletedItems)
                .HasColumnName("TaskProgressCompleted")
                .IsRequired();
            
            p.Property(x => x.TotalItems)
                .HasColumnName("TaskProgressTotal")
                .IsRequired();
        });

        // Relationships
        builder.HasMany(t => t.Subtasks)
            .WithOne()
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey("TaskId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Attachments)
            .WithOne()
            .HasForeignKey("TaskId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_ProjectTasks_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(t => new { t.ProjectId, t.Status })
            .HasDatabaseName("IX_ProjectTasks_ProjectId_Status")
            .HasFillFactor(90);

        builder.HasIndex(t => new { t.ProjectId, t.AssigneeId })
            .HasDatabaseName("IX_ProjectTasks_ProjectId_AssigneeId")
            .HasFillFactor(90);

        builder.HasIndex(t => t.ParentTaskId)
            .HasDatabaseName("IX_ProjectTasks_ParentTaskId")
            .HasFillFactor(90);

        builder.HasIndex(t => t.MilestoneId)
            .HasDatabaseName("IX_ProjectTasks_MilestoneId")
            .HasFillFactor(90);

        builder.HasIndex(t => t.TaskNumber)
            .IsUnique()
            .HasDatabaseName("IX_ProjectTasks_TaskNumber_Unique")
            .HasFillFactor(95);

        // Ignore navigation properties
        builder.Ignore(t => t.DomainEvents);
    }
}
