using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Aggregates;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Project aggregate
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table configuration
        builder.ToTable("Projects", "projects");
        
        // Primary key
        builder.HasKey(p => p.Id);
        
        // Properties
        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.DeliveryMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.BillingType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.ProjectManagerId)
            .IsRequired();

        builder.Property(p => p.ProjectManagerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.ClientId);

        builder.Property(p => p.ClientName)
            .HasMaxLength(255);

        builder.Property(p => p.ClientType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(p => p.StartedAt);
        builder.Property(p => p.CompletedAt);
        builder.Property(p => p.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(p => p.ProjectCode, pc =>
        {
            pc.Property(x => x.Value)
                .HasColumnName("ProjectCode")
                .IsRequired()
                .HasMaxLength(50);
        });

        builder.ComplexProperty(p => p.Priority, pr =>
        {
            pr.Property(x => x.Level)
                .HasColumnName("Priority")
                .IsRequired()
                .HasMaxLength(50);
        });

        builder.ComplexProperty(p => p.Budget, b =>
        {
            b.Property(x => x.Amount)
                .HasColumnName("BudgetAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            b.Property(x => x.Currency)
                .HasColumnName("BudgetCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.ActualCost, ac =>
        {
            ac.Property(x => x.Amount)
                .HasColumnName("ActualCostAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            ac.Property(x => x.Currency)
                .HasColumnName("ActualCostCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(p => p.OverallProgress, op =>
        {
            op.Property(x => x.CompletedItems)
                .HasColumnName("ProgressCompleted")
                .IsRequired();
            
            op.Property(x => x.TotalItems)
                .HasColumnName("ProgressTotal")
                .IsRequired();
        });

        builder.ComplexProperty(p => p.Timeline, t =>
        {
            t.Property(x => x.StartDate)
                .HasColumnName("TimelineStartDate");
            
            t.Property(x => x.EndDate)
                .HasColumnName("TimelineEndDate");
        });

        builder.ComplexProperty(p => p.TeamCapacity, tc =>
        {
            tc.Property(x => x.TotalMembers)
                .HasColumnName("TeamCapacityMembers");
            
            tc.Property(x => x.TotalCapacity)
                .HasColumnName("TeamCapacityTotal")
                .HasColumnType("decimal(5,2)");
        });

        // Relationships
        builder.HasMany(p => p.Tasks)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Milestones)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ResourceAllocations)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.TimeEntries)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Risks)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Issues)
            .WithOne()
            .HasForeignKey("ProjectId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes with EF Core 9 fill factors for performance optimization
        builder.HasIndex(p => p.TenantId)
            .HasDatabaseName("IX_Projects_TenantId")
            .HasFillFactor(90);

        builder.HasIndex(p => new { p.TenantId, p.Status })
            .HasDatabaseName("IX_Projects_TenantId_Status")
            .HasFillFactor(90);

        builder.HasIndex(p => new { p.TenantId, p.ProjectManagerId })
            .HasDatabaseName("IX_Projects_TenantId_ProjectManagerId")
            .HasFillFactor(90);

        builder.HasIndex(p => new { p.TenantId, p.ClientId })
            .HasDatabaseName("IX_Projects_TenantId_ClientId")
            .HasFillFactor(90);

        builder.ComplexProperty(p => p.ProjectCode)
            .HasIndex(pc => pc.Value)
            .IsUnique()
            .HasDatabaseName("IX_Projects_ProjectCode_Unique")
            .HasFillFactor(95);

        // Ignore navigation properties that are part of the aggregate
        builder.Ignore(p => p.DomainEvents);
    }
}
