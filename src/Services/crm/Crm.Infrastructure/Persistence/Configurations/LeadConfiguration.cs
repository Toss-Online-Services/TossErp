using TossErp.CRM.Domain.Aggregates;
using TossErp.CRM.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        // Table configuration
        builder.ToTable("Leads");

        // Primary key
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .IsRequired()
            .ValueGeneratedNever();

        // Basic properties
        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.Property(l => l.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Company)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.JobTitle)
            .HasMaxLength(100);

        builder.Property(l => l.Industry)
            .HasMaxLength(100);

        builder.Property(l => l.CompanySize);

        // Value Objects
        builder.OwnsOne(l => l.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email");
        });

        builder.OwnsOne(l => l.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasMaxLength(20)
                .HasColumnName("Phone");
        });

        builder.OwnsOne(l => l.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasMaxLength(200)
                .HasColumnName("AddressStreet");
            addressBuilder.Property(a => a.City)
                .HasMaxLength(100)
                .HasColumnName("AddressCity");
            addressBuilder.Property(a => a.State)
                .HasMaxLength(100)
                .HasColumnName("AddressState");
            addressBuilder.Property(a => a.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("AddressPostalCode");
            addressBuilder.Property(a => a.Country)
                .HasMaxLength(100)
                .HasColumnName("AddressCountry");
        });

        builder.OwnsOne(l => l.Score, scoreBuilder =>
        {
            scoreBuilder.Property(s => s.Value)
                .IsRequired()
                .HasColumnName("Score");
        });

        builder.OwnsOne(l => l.EstimatedValue, valueBuilder =>
        {
            valueBuilder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("EstimatedValueAmount");
            valueBuilder.Property(e => e.Currency)
                .HasMaxLength(3)
                .HasColumnName("EstimatedValueCurrency");
        });

        // Enum properties
        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(l => l.Source)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        // Assignment and tracking
        builder.Property(l => l.AssignedTo)
            .HasMaxLength(100);

        // Audit properties
        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.ModifiedAt);

        builder.Property(l => l.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(l => l.LastContactedAt);

        builder.Property(l => l.QualifiedAt);

        builder.Property(l => l.QualifiedBy)
            .HasMaxLength(100);

        // Conversion tracking
        builder.Property(l => l.ConvertedAt);

        builder.Property(l => l.ConvertedCustomerId);

        builder.Property(l => l.ConvertedOpportunityId);

        builder.Property(l => l.ConvertedBy)
            .HasMaxLength(100);

        // Campaign tracking
        builder.Property(l => l.CampaignId);

        builder.Property(l => l.CampaignName)
            .HasMaxLength(200);

        // Additional properties
        builder.Property(l => l.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(l => l.Remarks)
            .HasMaxLength(2000);

        builder.Property(l => l.NextFollowUp);

        builder.Property(l => l.ContactAttempts)
            .HasDefaultValue(0);

        builder.Property(l => l.LastActivityDate);

        builder.Property(l => l.IsDeleted)
            .HasDefaultValue(false);

        // Ignore computed properties
        builder.Ignore(l => l.FullName);
        builder.Ignore(l => l.IsQualified);
        builder.Ignore(l => l.IsConverted);
        builder.Ignore(l => l.DaysInPipeline);
        builder.Ignore(l => l.DaysSinceLastContact);
        builder.Ignore(l => l.IsStale);

        // Ignore collections for now (will be configured separately if needed)
        builder.Ignore(l => l.Activities);
        builder.Ignore(l => l.Notes);
        builder.Ignore(l => l.Communications);

        // Indexes
        builder.HasIndex(l => l.TenantId)
            .HasDatabaseName("IX_Lead_TenantId");

        builder.HasIndex(l => l.Status)
            .HasDatabaseName("IX_Lead_Status");

        builder.HasIndex(l => l.Source)
            .HasDatabaseName("IX_Lead_Source");

        builder.HasIndex(l => l.AssignedTo)
            .HasDatabaseName("IX_Lead_AssignedTo");

        builder.HasIndex(l => l.CreatedAt)
            .HasDatabaseName("IX_Lead_CreatedAt");

        builder.HasIndex(l => l.LastContactedAt)
            .HasDatabaseName("IX_Lead_LastContactedAt");

        builder.HasIndex(l => l.QualifiedAt)
            .HasDatabaseName("IX_Lead_QualifiedAt");

        builder.HasIndex(l => l.ConvertedAt)
            .HasDatabaseName("IX_Lead_ConvertedAt");

        builder.HasIndex(l => l.CampaignId)
            .HasDatabaseName("IX_Lead_CampaignId");

        builder.HasIndex(l => l.IsDeleted)
            .HasDatabaseName("IX_Lead_IsDeleted");

        // Composite indexes
        builder.HasIndex(l => new { l.TenantId, l.Status })
            .HasDatabaseName("IX_Lead_Tenant_Status");

        builder.HasIndex(l => new { l.TenantId, l.AssignedTo })
            .HasDatabaseName("IX_Lead_Tenant_AssignedTo");
    }
}
