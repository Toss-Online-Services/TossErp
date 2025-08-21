using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class ApiKeyConfigurationConfiguration : IEntityTypeConfiguration<ApiKeyConfiguration>
{
    public void Configure(EntityTypeBuilder<ApiKeyConfiguration> builder)
    {
        builder.ToTable("ApiKeyConfigurations");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.KeyHash)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(a => a.KeyPrefix)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.ExpiresAt);

        builder.Property(a => a.LastUsedAt);

        builder.Property(a => a.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(a => a.Owner)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(a => a.LastModifiedAt);

        builder.Property(a => a.Environment)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.RevokedAt);

        builder.Property(a => a.RevokedBy)
            .HasMaxLength(100);

        builder.Property(a => a.RevokedReason)
            .HasMaxLength(500);

        // Configure scopes as JSON
        builder.Property(a => a.Scopes)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure allowed IPs as JSON
        builder.Property(a => a.AllowedIpAddresses)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure allowed referrers as JSON
        builder.Property(a => a.AllowedReferrers)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure custom claims as JSON
        builder.Property(a => a.CustomClaims)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(a => a.KeyHash)
            .IsUnique()
            .HasDatabaseName("IX_ApiKeyConfigurations_KeyHash")
            .HasFillFactor(90);

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("IX_ApiKeyConfigurations_Name")
            .HasFillFactor(85);

        builder.HasIndex(a => a.IsEnabled)
            .HasDatabaseName("IX_ApiKeyConfigurations_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(a => a.Owner)
            .HasDatabaseName("IX_ApiKeyConfigurations_Owner")
            .HasFillFactor(85);

        builder.HasIndex(a => a.Environment)
            .HasDatabaseName("IX_ApiKeyConfigurations_Environment")
            .HasFillFactor(85);

        builder.HasIndex(a => new { a.IsEnabled, a.IsRevoked })
            .HasDatabaseName("IX_ApiKeyConfigurations_IsEnabled_IsRevoked")
            .HasFillFactor(85);

        builder.HasIndex(a => a.ExpiresAt)
            .HasDatabaseName("IX_ApiKeyConfigurations_ExpiresAt")
            .HasFillFactor(85);

        builder.HasIndex(a => new { a.Owner, a.IsEnabled })
            .HasDatabaseName("IX_ApiKeyConfigurations_Owner_IsEnabled")
            .HasFillFactor(85);
    }
}
