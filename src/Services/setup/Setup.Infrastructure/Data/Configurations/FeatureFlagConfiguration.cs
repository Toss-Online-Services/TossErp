using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("FeatureFlags");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.Property(f => f.IsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.EnabledForPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(f => f.Environment)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(f => f.StartDate);

        builder.Property(f => f.EndDate);

        builder.Property(f => f.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(f => f.LastModifiedAt);

        builder.Property(f => f.Tags)
            .HasMaxLength(500);

        builder.Property(f => f.Owner)
            .HasMaxLength(100);

        builder.Property(f => f.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure enabled users as JSON
        builder.Property(f => f.EnabledUsers)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure enabled roles as JSON
        builder.Property(f => f.EnabledRoles)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure enabled tenants as JSON
        builder.Property(f => f.EnabledTenants)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure conditions as JSON
        builder.Property(f => f.Conditions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Configure variants as JSON
        builder.Property(f => f.Variants)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(f => f.Key)
            .IsUnique()
            .HasDatabaseName("IX_FeatureFlags_Key")
            .HasFillFactor(90);

        builder.HasIndex(f => f.IsEnabled)
            .HasDatabaseName("IX_FeatureFlags_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(f => f.Environment)
            .HasDatabaseName("IX_FeatureFlags_Environment")
            .HasFillFactor(85);

        builder.HasIndex(f => new { f.IsEnabled, f.Environment })
            .HasDatabaseName("IX_FeatureFlags_IsEnabled_Environment")
            .HasFillFactor(85);

        builder.HasIndex(f => f.Owner)
            .HasDatabaseName("IX_FeatureFlags_Owner")
            .HasFillFactor(85);

        builder.HasIndex(f => f.IsArchived)
            .HasDatabaseName("IX_FeatureFlags_IsArchived")
            .HasFillFactor(85);

        builder.HasIndex(f => new { f.StartDate, f.EndDate })
            .HasDatabaseName("IX_FeatureFlags_StartDate_EndDate")
            .HasFillFactor(85);
    }
}
