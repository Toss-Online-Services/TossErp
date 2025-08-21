using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Entities;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ResourceAllocation entity
/// </summary>
public class ResourceAllocationConfiguration : IEntityTypeConfiguration<ResourceAllocation>
{
    public void Configure(EntityTypeBuilder<ResourceAllocation> builder)
    {
        // Table configuration
        builder.ToTable("ResourceAllocations", "projects");
        
        // Primary key
        builder.HasKey(ra => ra.Id);
        
        // Properties
        builder.Property(ra => ra.Id)
            .IsRequired();

        builder.Property(ra => ra.ProjectId)
            .IsRequired();

        builder.Property(ra => ra.ResourceId)
            .IsRequired();

        builder.Property(ra => ra.ResourceName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ra => ra.ResourceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ra => ra.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ra => ra.AllocationPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(ra => ra.AllocatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ra => ra.AllocatedAt)
            .IsRequired();

        builder.Property(ra => ra.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(ra => ra.ModifiedAt);

        builder.Property(ra => ra.Notes)
            .HasMaxLength(1000);

        // Complex types for EF Core 9
        builder.ComplexProperty(ra => ra.AllocationPeriod, ap =>
        {
            ap.Property(x => x.StartDate)
                .HasColumnName("AllocationStartDate")
                .IsRequired();
            
            ap.Property(x => x.EndDate)
                .HasColumnName("AllocationEndDate")
                .IsRequired();
        });

        builder.ComplexProperty(ra => ra.Cost, c =>
        {
            c.Property(x => x.Amount)
                .HasColumnName("CostAmount")
                .HasColumnType("decimal(18,2)");
            
            c.Property(x => x.Currency)
                .HasColumnName("CostCurrency")
                .HasMaxLength(3);
        });

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(ra => ra.ProjectId)
            .HasDatabaseName("IX_ResourceAllocations_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(ra => new { ra.ProjectId, ra.ResourceId })
            .HasDatabaseName("IX_ResourceAllocations_ProjectId_ResourceId")
            .HasFillFactor(90);

        builder.HasIndex(ra => new { ra.ProjectId, ra.Status })
            .HasDatabaseName("IX_ResourceAllocations_ProjectId_Status")
            .HasFillFactor(90);

        builder.HasIndex(ra => ra.ResourceId)
            .HasDatabaseName("IX_ResourceAllocations_ResourceId")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(ra => ra.DomainEvents);
    }
}
