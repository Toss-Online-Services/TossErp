using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class ModuleConfigurationConfiguration : IEntityTypeConfiguration<ModuleConfiguration>
{
    public void Configure(EntityTypeBuilder<ModuleConfiguration> builder)
    {
        builder.ToTable("ModuleConfigurations");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.ModuleName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.Version)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.IsCore)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.LoadOrder)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(m => m.Category)
            .HasMaxLength(50);

        builder.Property(m => m.Author)
            .HasMaxLength(200);

        builder.Property(m => m.LicenseType)
            .HasMaxLength(50);

        builder.Property(m => m.LicenseKey)
            .HasMaxLength(500);

        builder.Property(m => m.InstallDate)
            .IsRequired();

        builder.Property(m => m.LastUpdateDate);

        builder.Property(m => m.ExpiryDate);

        builder.Property(m => m.SupportUrl)
            .HasMaxLength(500);

        builder.Property(m => m.DocumentationUrl)
            .HasMaxLength(500);

        builder.Property(m => m.IconUrl)
            .HasMaxLength(500);

        builder.Property(m => m.AssemblyPath)
            .HasMaxLength(500);

        builder.Property(m => m.EntryPoint)
            .HasMaxLength(200);

        builder.Property(m => m.MinFrameworkVersion)
            .HasMaxLength(20);

        builder.Property(m => m.MaxFrameworkVersion)
            .HasMaxLength(20);

        // Complex type for configuration value
        builder.ComplexProperty(m => m.ConfigurationValue, cv =>
        {
            cv.Property(v => v.Key)
              .HasMaxLength(100)
              .IsRequired();
            
            cv.Property(v => v.Value)
              .HasMaxLength(2000);
            
            cv.Property(v => v.DataType)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
            
            cv.Property(v => v.IsEncrypted)
              .IsRequired()
              .HasDefaultValue(false);
            
            cv.Property(v => v.IsReadOnly)
              .IsRequired()
              .HasDefaultValue(false);
            
            cv.Property(v => v.DisplayName)
              .HasMaxLength(200);
            
            cv.Property(v => v.Description)
              .HasMaxLength(1000);
            
            cv.Property(v => v.DefaultValue)
              .HasMaxLength(2000);
            
            cv.Property(v => v.ValidationRegex)
              .HasMaxLength(500);
        });

        // Configure dependencies as JSON
        builder.Property(m => m.Dependencies)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure permissions as JSON
        builder.Property(m => m.Permissions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure menu items as JSON
        builder.Property(m => m.MenuItems)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure database tables as JSON
        builder.Property(m => m.DatabaseTables)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure configuration schema as JSON
        builder.Property(m => m.ConfigurationSchema)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(m => m.ModuleName)
            .IsUnique()
            .HasDatabaseName("IX_ModuleConfigurations_ModuleName")
            .HasFillFactor(90);

        builder.HasIndex(m => m.IsEnabled)
            .HasDatabaseName("IX_ModuleConfigurations_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(m => m.IsCore)
            .HasDatabaseName("IX_ModuleConfigurations_IsCore")
            .HasFillFactor(85);

        builder.HasIndex(m => m.LoadOrder)
            .HasDatabaseName("IX_ModuleConfigurations_LoadOrder")
            .HasFillFactor(85);

        builder.HasIndex(m => m.Category)
            .HasDatabaseName("IX_ModuleConfigurations_Category")
            .HasFillFactor(85);

        builder.HasIndex(m => new { m.IsEnabled, m.LoadOrder })
            .HasDatabaseName("IX_ModuleConfigurations_IsEnabled_LoadOrder")
            .HasFillFactor(85);
    }
}
