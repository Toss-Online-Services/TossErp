using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class ComplianceFrameworkConfiguration : IEntityTypeConfiguration<ComplianceFramework>
{
    public void Configure(EntityTypeBuilder<ComplianceFramework> builder)
    {
        builder.ToTable("ComplianceFrameworks");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.FrameworkType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Version)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.IsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ImplementationDate);

        builder.Property(c => c.CertificationDate);

        builder.Property(c => c.ExpiryDate);

        builder.Property(c => c.RenewalDate);

        builder.Property(c => c.AuditFrequency)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.LastAuditDate);

        builder.Property(c => c.NextAuditDate);

        builder.Property(c => c.CertifyingBody)
            .HasMaxLength(200);

        builder.Property(c => c.CertificateNumber)
            .HasMaxLength(100);

        builder.Property(c => c.AuditorName)
            .HasMaxLength(200);

        builder.Property(c => c.AuditorEmail)
            .HasMaxLength(256);

        builder.Property(c => c.ComplianceScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(c => c.RequiredScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(80);

        builder.Property(c => c.IsCompliant)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.NonComplianceReason)
            .HasMaxLength(1000);

        builder.Property(c => c.RemediationPlan)
            .HasMaxLength(2000);

        builder.Property(c => c.RemediationDeadline);

        // Complex type for compliance requirement
        builder.ComplexProperty(c => c.ComplianceRequirement, cr =>
        {
            cr.Property(r => r.RequirementId)
              .HasMaxLength(50)
              .IsRequired();
            
            cr.Property(r => r.Category)
              .HasMaxLength(100)
              .IsRequired();
            
            cr.Property(r => r.Description)
              .HasMaxLength(1000)
              .IsRequired();
            
            cr.Property(r => r.Priority)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
            
            cr.Property(r => r.Status)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
            
            cr.Property(r => r.DueDate);
            
            cr.Property(r => r.CompletedDate);
            
            cr.Property(r => r.Evidence)
              .HasMaxLength(2000);
            
            cr.Property(r => r.ResponsiblePerson)
              .HasMaxLength(200);
        });

        // Configure requirements as JSON
        builder.Property(c => c.Requirements)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure control mappings as JSON
        builder.Property(c => c.ControlMappings)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Configure evidence documents as JSON
        builder.Property(c => c.EvidenceDocuments)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(c => c.FrameworkType)
            .HasDatabaseName("IX_ComplianceFrameworks_FrameworkType")
            .HasFillFactor(85);

        builder.HasIndex(c => c.IsEnabled)
            .HasDatabaseName("IX_ComplianceFrameworks_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(c => c.IsCompliant)
            .HasDatabaseName("IX_ComplianceFrameworks_IsCompliant")
            .HasFillFactor(85);

        builder.HasIndex(c => new { c.IsEnabled, c.IsCompliant })
            .HasDatabaseName("IX_ComplianceFrameworks_IsEnabled_IsCompliant")
            .HasFillFactor(85);

        builder.HasIndex(c => c.ExpiryDate)
            .HasDatabaseName("IX_ComplianceFrameworks_ExpiryDate")
            .HasFillFactor(85);

        builder.HasIndex(c => c.NextAuditDate)
            .HasDatabaseName("IX_ComplianceFrameworks_NextAuditDate")
            .HasFillFactor(85);

        builder.HasIndex(c => c.CertificateNumber)
            .HasDatabaseName("IX_ComplianceFrameworks_CertificateNumber")
            .HasFillFactor(85);
    }
}
