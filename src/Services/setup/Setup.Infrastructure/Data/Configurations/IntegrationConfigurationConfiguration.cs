using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class IntegrationConfigurationConfiguration : IEntityTypeConfiguration<IntegrationConfiguration>
{
    public void Configure(EntityTypeBuilder<IntegrationConfiguration> builder)
    {
        builder.ToTable("IntegrationConfigurations");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.IntegrationType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.IsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.ApiEndpoint)
            .HasMaxLength(500);

        builder.Property(i => i.ApiKey)
            .HasMaxLength(500);

        builder.Property(i => i.ApiSecret)
            .HasMaxLength(500);

        builder.Property(i => i.AuthenticationType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.RefreshToken)
            .HasMaxLength(1000);

        builder.Property(i => i.TokenExpiresAt);

        builder.Property(i => i.WebhookUrl)
            .HasMaxLength(500);

        builder.Property(i => i.WebhookSecret)
            .HasMaxLength(200);

        builder.Property(i => i.IsWebhookEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.SyncFrequency)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.LastSyncAt);

        builder.Property(i => i.NextSyncAt);

        builder.Property(i => i.SyncStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.LastSyncError)
            .HasMaxLength(2000);

        builder.Property(i => i.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.MaxRetries)
            .IsRequired()
            .HasDefaultValue(3);

        // Configure settings as JSON
        builder.Property(i => i.Settings)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Configure mapping rules as JSON
        builder.Property(i => i.MappingRules)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(i => i.IntegrationType)
            .HasDatabaseName("IX_IntegrationConfigurations_IntegrationType")
            .HasFillFactor(85);

        builder.HasIndex(i => i.IsEnabled)
            .HasDatabaseName("IX_IntegrationConfigurations_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(i => new { i.IsEnabled, i.SyncStatus })
            .HasDatabaseName("IX_IntegrationConfigurations_IsEnabled_SyncStatus")
            .HasFillFactor(85);

        builder.HasIndex(i => i.NextSyncAt)
            .HasDatabaseName("IX_IntegrationConfigurations_NextSyncAt")
            .HasFillFactor(85);

        builder.HasIndex(i => new { i.IntegrationType, i.IsEnabled })
            .HasDatabaseName("IX_IntegrationConfigurations_IntegrationType_IsEnabled")
            .HasFillFactor(85);
    }
}
