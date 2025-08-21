using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class RateLimitRuleConfiguration : IEntityTypeConfiguration<RateLimitRule>
{
    public void Configure(EntityTypeBuilder<RateLimitRule> builder)
    {
        builder.ToTable("RateLimitRules");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Endpoint)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.HttpMethod)
            .HasMaxLength(10);

        builder.Property(r => r.ClientType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.Priority)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(r => r.QuotaExceededMessage)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(r => r.LastModifiedAt);

        // Complex type for rate limit
        builder.ComplexProperty(r => r.RateLimit, rl =>
        {
            rl.Property(l => l.RequestsPerWindow)
              .IsRequired();
            
            rl.Property(l => l.WindowSizeSeconds)
              .IsRequired();
            
            rl.Property(l => l.BurstLimit)
              .IsRequired();
            
            rl.Property(l => l.TimeoutSeconds)
              .IsRequired()
              .HasDefaultValue(30);
            
            rl.Property(l => l.ConcurrentRequestLimit)
              .IsRequired()
              .HasDefaultValue(10);
            
            rl.Property(l => l.SlidingWindow)
              .IsRequired()
              .HasDefaultValue(true);
            
            rl.Property(l => l.ResetMode)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
        });

        // Configure client identifiers as JSON
        builder.Property(r => r.ClientIdentifiers)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure excluded endpoints as JSON
        builder.Property(r => r.ExcludedEndpoints)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure whitelisted IPs as JSON
        builder.Property(r => r.WhitelistedIpAddresses)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure blacklisted IPs as JSON
        builder.Property(r => r.BlacklistedIpAddresses)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure conditions as JSON
        builder.Property(r => r.Conditions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_RateLimitRules_Name")
            .HasFillFactor(90);

        builder.HasIndex(r => r.Endpoint)
            .HasDatabaseName("IX_RateLimitRules_Endpoint")
            .HasFillFactor(85);

        builder.HasIndex(r => r.IsEnabled)
            .HasDatabaseName("IX_RateLimitRules_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(r => r.ClientType)
            .HasDatabaseName("IX_RateLimitRules_ClientType")
            .HasFillFactor(85);

        builder.HasIndex(r => new { r.IsEnabled, r.Priority })
            .HasDatabaseName("IX_RateLimitRules_IsEnabled_Priority")
            .HasFillFactor(85);

        builder.HasIndex(r => new { r.Endpoint, r.HttpMethod })
            .HasDatabaseName("IX_RateLimitRules_Endpoint_HttpMethod")
            .HasFillFactor(85);

        builder.HasIndex(r => new { r.ClientType, r.IsEnabled })
            .HasDatabaseName("IX_RateLimitRules_ClientType_IsEnabled")
            .HasFillFactor(85);
    }
}
