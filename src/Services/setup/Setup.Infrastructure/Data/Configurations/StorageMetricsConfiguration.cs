using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class StorageMetricsConfiguration : IEntityTypeConfiguration<StorageMetrics>
{
    public void Configure(EntityTypeBuilder<StorageMetrics> builder)
    {
        builder.ToTable("StorageMetrics");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.RecordedAt)
            .IsRequired();

        builder.Property(s => s.DatabaseSizeBytes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.FileStorageSizeBytes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.BackupSizeBytes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.TotalSizeBytes)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.DocumentCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ImageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.VideoCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.OtherFileCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ActiveUsersCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.TransactionsCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ApiCallsCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.EmailsSentCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ReportsGeneratedCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.WorkflowsExecutedCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.StorageQuotaUtilization)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.UserQuotaUtilization)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ApiQuotaUtilization)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasDefaultValue(0);

        // Configure breakdown by service as JSON
        builder.Property(s => s.BreakdownByService)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, long>())
            .HasColumnType("nvarchar(max)");

        // Configure breakdown by file type as JSON
        builder.Property(s => s.BreakdownByFileType)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, long>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, long>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(s => s.RecordedAt)
            .HasDatabaseName("IX_StorageMetrics_RecordedAt")
            .HasFillFactor(85);

        builder.HasIndex(s => new { s.RecordedAt, s.TotalSizeBytes })
            .HasDatabaseName("IX_StorageMetrics_RecordedAt_TotalSizeBytes")
            .HasFillFactor(85);

        builder.HasIndex(s => s.StorageQuotaUtilization)
            .HasDatabaseName("IX_StorageMetrics_StorageQuotaUtilization")
            .HasFillFactor(85);

        builder.HasIndex(s => s.UserQuotaUtilization)
            .HasDatabaseName("IX_StorageMetrics_UserQuotaUtilization")
            .HasFillFactor(85);

        builder.HasIndex(s => s.ApiQuotaUtilization)
            .HasDatabaseName("IX_StorageMetrics_ApiQuotaUtilization")
            .HasFillFactor(85);
    }
}
