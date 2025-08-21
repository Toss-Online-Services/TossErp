using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.OrganizationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(1000);

        builder.Property(o => o.Industry)
            .HasMaxLength(100);

        builder.Property(o => o.Website)
            .HasMaxLength(500);

        builder.Property(o => o.EmployeeCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.AnnualRevenue)
            .HasPrecision(18, 2);

        builder.Property(o => o.Currency)
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(o => o.FiscalYearStartMonth)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(o => o.TimeZone)
            .HasMaxLength(50)
            .HasDefaultValue("UTC");

        builder.Property(o => o.DefaultLanguage)
            .HasMaxLength(10)
            .HasDefaultValue("en-US");

        builder.Property(o => o.DateFormat)
            .HasMaxLength(20)
            .HasDefaultValue("yyyy-MM-dd");

        builder.Property(o => o.NumberFormat)
            .HasMaxLength(20)
            .HasDefaultValue("en-US");

        builder.Property(o => o.LogoUrl)
            .HasMaxLength(500);

        builder.Property(o => o.EstablishedDate);

        builder.Property(o => o.RegistrationNumber)
            .HasMaxLength(100);

        builder.Property(o => o.TaxNumber)
            .HasMaxLength(100);

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Complex type for primary contact info
        builder.ComplexProperty(o => o.PrimaryContactInfo, pci =>
        {
            pci.Property(c => c.Name)
               .HasMaxLength(200);
            
            pci.Property(c => c.Title)
               .HasMaxLength(100);
            
            pci.Property(c => c.Email)
               .HasMaxLength(256);
            
            pci.Property(c => c.Phone)
               .HasMaxLength(20);
            
            pci.Property(c => c.Mobile)
               .HasMaxLength(20);
            
            pci.Property(c => c.Fax)
               .HasMaxLength(20);
        });

        // Complex type for primary address
        builder.ComplexProperty(o => o.PrimaryAddress, pa =>
        {
            pa.Property(a => a.Street1)
              .HasMaxLength(200);
            
            pa.Property(a => a.Street2)
              .HasMaxLength(200);
            
            pa.Property(a => a.City)
              .HasMaxLength(100);
            
            pa.Property(a => a.State)
              .HasMaxLength(100);
            
            pa.Property(a => a.PostalCode)
              .HasMaxLength(20);
            
            pa.Property(a => a.Country)
              .HasMaxLength(100);
            
            pa.Property(a => a.Latitude)
              .HasPrecision(10, 8);
            
            pa.Property(a => a.Longitude)
              .HasPrecision(11, 8);
        });

        // Complex type for security policy
        builder.ComplexProperty(o => o.SecurityPolicy, sp =>
        {
            sp.Property(p => p.RequirePasswordChange)
              .IsRequired()
              .HasDefaultValue(true);
            
            sp.Property(p => p.PasswordExpiryDays)
              .IsRequired()
              .HasDefaultValue(90);
            
            sp.Property(p => p.RequireTwoFactor)
              .IsRequired()
              .HasDefaultValue(false);
            
            sp.Property(p => p.AllowedIpAddresses)
              .HasMaxLength(1000);
            
            sp.Property(p => p.SessionTimeoutMinutes)
              .IsRequired()
              .HasDefaultValue(30);
            
            sp.Property(p => p.MaxConcurrentSessions)
              .IsRequired()
              .HasDefaultValue(5);
        });

        // Complex type for compliance requirement
        builder.ComplexProperty(o => o.ComplianceRequirement, cr =>
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

        // Configure departments as JSON
        builder.Property(o => o.Departments)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure locations as JSON
        builder.Property(o => o.Locations)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure subsidiaries as JSON
        builder.Property(o => o.Subsidiaries)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure business units as JSON
        builder.Property(o => o.BusinessUnits)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure cost centers as JSON
        builder.Property(o => o.CostCenters)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure custom fields as JSON
        builder.Property(o => o.CustomFields)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(o => o.Code)
            .IsUnique()
            .HasDatabaseName("IX_Organizations_Code")
            .HasFillFactor(90);

        builder.HasIndex(o => o.Name)
            .HasDatabaseName("IX_Organizations_Name")
            .HasFillFactor(85);

        builder.HasIndex(o => o.IsActive)
            .HasDatabaseName("IX_Organizations_IsActive")
            .HasFillFactor(85);

        builder.HasIndex(o => o.Industry)
            .HasDatabaseName("IX_Organizations_Industry")
            .HasFillFactor(85);

        builder.HasIndex(o => o.RegistrationNumber)
            .HasDatabaseName("IX_Organizations_RegistrationNumber")
            .HasFillFactor(85);

        builder.HasIndex(o => o.TaxNumber)
            .HasDatabaseName("IX_Organizations_TaxNumber")
            .HasFillFactor(85);
    }
}
