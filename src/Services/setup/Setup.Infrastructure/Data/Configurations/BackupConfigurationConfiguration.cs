using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class BackupConfigurationConfiguration : IEntityTypeConfiguration<BackupConfiguration>
{
    public void Configure(EntityTypeBuilder<BackupConfiguration> builder)
    {
        builder.ToTable("BackupConfigurations");
        
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.BackupType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Schedule)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.StorageLocation)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.EncryptionKey)
            .HasMaxLength(200);

        builder.Property(b => b.CompressionLevel)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(b => b.MaxBackupSizeGB)
            .HasPrecision(10, 2);

        builder.Property(b => b.LastBackupAt);

        builder.Property(b => b.NextBackupAt);

        builder.Property(b => b.LastBackupSize)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.LastBackupDuration)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.BackupCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.FailureCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.LastError)
            .HasMaxLength(2000);

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(b => b.LastModifiedAt);

        // Complex type for backup retention
        builder.ComplexProperty(b => b.BackupRetention, br =>
        {
            br.Property(r => r.DailyRetentionDays)
              .IsRequired()
              .HasDefaultValue(7);
            
            br.Property(r => r.WeeklyRetentionWeeks)
              .IsRequired()
              .HasDefaultValue(4);
            
            br.Property(r => r.MonthlyRetentionMonths)
              .IsRequired()
              .HasDefaultValue(12);
            
            br.Property(r => r.YearlyRetentionYears)
              .IsRequired()
              .HasDefaultValue(3);
            
            br.Property(r => r.MaxTotalBackups)
              .IsRequired()
              .HasDefaultValue(100);
            
            br.Property(r => r.AutoPurge)
              .IsRequired()
              .HasDefaultValue(true);
            
            br.Property(r => r.ArchiveAfterDays)
              .IsRequired()
              .HasDefaultValue(30);
        });

        // Configure included databases as JSON
        builder.Property(b => b.IncludedDatabases)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure excluded tables as JSON
        builder.Property(b => b.ExcludedTables)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure notification recipients as JSON
        builder.Property(b => b.NotificationRecipients)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure pre-backup scripts as JSON
        builder.Property(b => b.PreBackupScripts)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure post-backup scripts as JSON
        builder.Property(b => b.PostBackupScripts)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure advanced options as JSON
        builder.Property(b => b.AdvancedOptions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(b => b.Name)
            .IsUnique()
            .HasDatabaseName("IX_BackupConfigurations_Name")
            .HasFillFactor(90);

        builder.HasIndex(b => b.BackupType)
            .HasDatabaseName("IX_BackupConfigurations_BackupType")
            .HasFillFactor(85);

        builder.HasIndex(b => b.IsEnabled)
            .HasDatabaseName("IX_BackupConfigurations_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(b => b.NextBackupAt)
            .HasDatabaseName("IX_BackupConfigurations_NextBackupAt")
            .HasFillFactor(85);

        builder.HasIndex(b => new { b.IsEnabled, b.NextBackupAt })
            .HasDatabaseName("IX_BackupConfigurations_IsEnabled_NextBackupAt")
            .HasFillFactor(85);

        builder.HasIndex(b => b.LastBackupAt)
            .HasDatabaseName("IX_BackupConfigurations_LastBackupAt")
            .HasFillFactor(85);

        builder.HasIndex(b => new { b.BackupType, b.IsEnabled })
            .HasDatabaseName("IX_BackupConfigurations_BackupType_IsEnabled")
            .HasFillFactor(85);
    }
}
