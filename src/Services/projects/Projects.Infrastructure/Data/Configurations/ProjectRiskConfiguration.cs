using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Projects.Domain.Aggregates;

namespace TossErp.Projects.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ProjectRisk aggregate
/// </summary>
public class ProjectRiskConfiguration : IEntityTypeConfiguration<ProjectRisk>
{
    public void Configure(EntityTypeBuilder<ProjectRisk> builder)
    {
        // Table configuration
        builder.ToTable("ProjectRisks", "projects");
        
        // Primary key
        builder.HasKey(r => r.Id);
        
        // Properties
        builder.Property(r => r.Id)
            .IsRequired();

        builder.Property(r => r.ProjectId)
            .IsRequired();

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.MitigationPlan)
            .HasMaxLength(2000);

        builder.Property(r => r.ContingencyPlan)
            .HasMaxLength(2000);

        builder.Property(r => r.OwnerId);

        builder.Property(r => r.OwnerName)
            .HasMaxLength(255);

        builder.Property(r => r.IdentifiedDate)
            .IsRequired();

        builder.Property(r => r.TargetResolutionDate);
        builder.Property(r => r.ActualResolutionDate);

        builder.Property(r => r.IdentifiedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ModifiedBy)
            .HasMaxLength(255);

        builder.Property(r => r.ModifiedAt);

        // Complex types for EF Core 9
        builder.ComplexProperty(r => r.RiskScore, rs =>
        {
            rs.Property(x => x.Probability)
                .HasColumnName("RiskProbability")
                .IsRequired();
            
            rs.Property(x => x.Impact)
                .HasColumnName("RiskImpact")
                .IsRequired();
            
            rs.Property(x => x.RiskLevel)
                .HasColumnName("RiskLevel")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(r => r.ProjectId)
            .HasDatabaseName("IX_ProjectRisks_ProjectId")
            .HasFillFactor(90);

        builder.HasIndex(r => new { r.ProjectId, r.Status })
            .HasDatabaseName("IX_ProjectRisks_ProjectId_Status")
            .HasFillFactor(90);

        builder.HasIndex(r => new { r.ProjectId, r.OwnerId })
            .HasDatabaseName("IX_ProjectRisks_ProjectId_OwnerId")
            .HasFillFactor(90);

        builder.ComplexProperty(r => r.RiskScore)
            .HasIndex(rs => rs.RiskLevel)
            .HasDatabaseName("IX_ProjectRisks_RiskLevel")
            .HasFillFactor(90);

        // Ignore navigation properties
        builder.Ignore(r => r.DomainEvents);
    }
}
