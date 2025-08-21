using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class AuditConfigurationConfiguration : IEntityTypeConfiguration<AuditConfiguration>
{
    public void Configure(EntityTypeBuilder<AuditConfiguration> builder)
    {
        builder.ToTable("AuditConfigurations");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LogUserActions)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LogDataChanges)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LogSystemEvents)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LogApiCalls)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.LogFailedLogins)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.LogSuccessfulLogins)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.RetentionPeriodDays)
            .IsRequired()
            .HasDefaultValue(365);

        builder.Property(a => a.CompressOldLogs)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.ExportToExternalSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.ExternalSystemEndpoint)
            .HasMaxLength(500);

        builder.Property(a => a.ExternalSystemApiKey)
            .HasMaxLength(200);

        builder.Property(a => a.AlertOnSuspiciousActivity)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.AlertThresholdMinutes)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(a => a.MaxFailedLoginsBeforeAlert)
            .IsRequired()
            .HasDefaultValue(5);

        // Configure excluded events as JSON
        builder.Property(a => a.ExcludedEvents)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure included tables as JSON
        builder.Property(a => a.IncludedTables)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure alert recipients as JSON
        builder.Property(a => a.AlertRecipients)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(a => a.IsEnabled)
            .HasDatabaseName("IX_AuditConfigurations_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(a => a.AlertOnSuspiciousActivity)
            .HasDatabaseName("IX_AuditConfigurations_AlertOnSuspiciousActivity")
            .HasFillFactor(85);

        builder.HasIndex(a => new { a.IsEnabled, a.LogUserActions })
            .HasDatabaseName("IX_AuditConfigurations_IsEnabled_LogUserActions")
            .HasFillFactor(85);
    }
}
