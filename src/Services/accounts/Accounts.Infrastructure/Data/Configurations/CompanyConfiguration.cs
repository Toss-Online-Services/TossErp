using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Accounts.Domain.Entities;

namespace TossErp.Accounts.Infrastructure.Data.Configurations;

/// <summary>
/// Company entity configuration for ERPNext-compliant Company management
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", "accounts");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired()
            .HasMaxLength(50);

        // Basic Company Information
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Abbreviation)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Domain)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        // Company Type and Status
        builder.Property(e => e.IsGroup)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Hierarchy
        builder.Property(e => e.ParentCompanyId);

        // Financial Information
        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(e => e.TaxId)
            .HasMaxLength(50);

        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(50);

        // Address Information
        builder.Property(e => e.Address)
            .HasMaxLength(1000);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .HasMaxLength(100);

        builder.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("United States");

        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);

        // Contact Information
        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.Website)
            .HasMaxLength(200);

        builder.Property(e => e.Fax)
            .HasMaxLength(20);

        // Chart of Accounts Settings
        builder.Property(e => e.ChartOfAccountsName)
            .HasMaxLength(200);

        builder.Property(e => e.ChartOfAccountsBasedOn)
            .HasMaxLength(200);

        builder.Property(e => e.CreateChartOfAccountsBasedOn)
            .IsRequired()
            .HasDefaultValue(true);

        // Financial Year Settings
        builder.Property(e => e.FinancialYearStartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(e => e.FinancialYearEndDate)
            .IsRequired()
            .HasColumnType("date");

        // Default Accounts (stored as JSON for flexibility)
        builder.Property(e => e.DefaultAccounts)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        // Taxes and Compliance
        builder.Property(e => e.TaxCategories)
            .HasColumnType("jsonb")
            .HasDefaultValue("[]");

        builder.Property(e => e.ComplianceSettings)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        // Additional Settings
        builder.Property(e => e.Settings)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Self-referencing relationship for company hierarchy
        builder.HasOne(e => e.ParentCompany)
            .WithMany(e => e.ChildCompanies)
            .HasForeignKey(e => e.ParentCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique Constraints
        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasDatabaseName("IX_Companies_TenantId_Name_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.Abbreviation })
            .IsUnique()
            .HasDatabaseName("IX_Companies_TenantId_Abbreviation_Unique")
            .HasFillFactor(95);

        // Performance Indexes
        builder.HasIndex(e => new { e.TenantId, e.IsActive })
            .HasDatabaseName("IX_Companies_TenantId_IsActive")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.IsGroup })
            .HasDatabaseName("IX_Companies_TenantId_IsGroup")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.ParentCompanyId })
            .HasDatabaseName("IX_Companies_TenantId_ParentCompanyId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Currency })
            .HasDatabaseName("IX_Companies_TenantId_Currency")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.Country })
            .HasDatabaseName("IX_Companies_TenantId_Country")
            .HasFillFactor(90);

        // Domain/Email filtering index
        builder.HasIndex(e => new { e.TenantId, e.Domain })
            .HasDatabaseName("IX_Companies_TenantId_Domain")
            .HasFillFactor(90);

        // Registration number index for compliance
        builder.HasIndex(e => new { e.TenantId, e.RegistrationNumber })
            .HasDatabaseName("IX_Companies_TenantId_RegistrationNumber")
            .HasFillFactor(90);

        // Tax ID index for compliance
        builder.HasIndex(e => new { e.TenantId, e.TaxId })
            .HasDatabaseName("IX_Companies_TenantId_TaxId")
            .HasFillFactor(90);
    }
}
