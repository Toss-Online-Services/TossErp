using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class ApplicationConfigurationConfiguration : IEntityTypeConfiguration<ApplicationConfiguration>
{
    public void Configure(EntityTypeBuilder<ApplicationConfiguration> builder)
    {
        builder.ToTable("ApplicationConfigurations");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.ApplicationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Version)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Environment)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.InstanceName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.MaintenanceMode)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.MaintenanceMessage)
            .HasMaxLength(500);

        builder.Property(a => a.MaintenanceStartTime);

        builder.Property(a => a.MaintenanceEndTime);

        builder.Property(a => a.AllowRegistration)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.RequireEmailVerification)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.DefaultUserRole)
            .HasMaxLength(50)
            .HasDefaultValue("User");

        builder.Property(a => a.SessionTimeoutMinutes)
            .IsRequired()
            .HasDefaultValue(30);

        builder.Property(a => a.MaxFileUploadSizeMB)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(a => a.SupportEmail)
            .HasMaxLength(256);

        builder.Property(a => a.SupportPhone)
            .HasMaxLength(20);

        builder.Property(a => a.CompanyName)
            .HasMaxLength(200);

        builder.Property(a => a.CompanyAddress)
            .HasMaxLength(500);

        builder.Property(a => a.CompanyWebsite)
            .HasMaxLength(500);

        builder.Property(a => a.PrivacyPolicyUrl)
            .HasMaxLength(500);

        builder.Property(a => a.TermsOfServiceUrl)
            .HasMaxLength(500);

        builder.Property(a => a.CopyrightText)
            .HasMaxLength(200);

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LastModifiedAt)
            .IsRequired();

        builder.Property(a => a.LastModifiedBy)
            .HasMaxLength(100)
            .IsRequired();

        // Configure allowed file extensions as JSON
        builder.Property(a => a.AllowedFileExtensions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure supported languages as JSON
        builder.Property(a => a.SupportedLanguages)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure supported currencies as JSON
        builder.Property(a => a.SupportedCurrencies)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure custom settings as JSON
        builder.Property(a => a.CustomSettings)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Configure relationships
        builder.HasMany<ModuleConfiguration>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<FeatureFlag>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<NotificationTemplate>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<ApiKeyConfiguration>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<RateLimitRule>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<BackupConfiguration>()
            .WithOne()
            .HasForeignKey("ApplicationConfigurationId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(a => a.ApplicationName)
            .HasDatabaseName("IX_ApplicationConfigurations_ApplicationName")
            .HasFillFactor(85);

        builder.HasIndex(a => a.Environment)
            .HasDatabaseName("IX_ApplicationConfigurations_Environment")
            .HasFillFactor(85);

        builder.HasIndex(a => a.InstanceName)
            .HasDatabaseName("IX_ApplicationConfigurations_InstanceName")
            .HasFillFactor(85);

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("IX_ApplicationConfigurations_IsActive")
            .HasFillFactor(85);

        builder.HasIndex(a => a.MaintenanceMode)
            .HasDatabaseName("IX_ApplicationConfigurations_MaintenanceMode")
            .HasFillFactor(85);

        builder.HasIndex(a => new { a.ApplicationName, a.Environment, a.InstanceName })
            .IsUnique()
            .HasDatabaseName("IX_ApplicationConfigurations_ApplicationName_Environment_InstanceName")
            .HasFillFactor(90);
    }
}
