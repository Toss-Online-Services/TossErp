using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.ApplicationConfigurationAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");
        
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(n => n.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Category)
            .HasMaxLength(50);

        builder.Property(n => n.Subject)
            .HasMaxLength(500);

        builder.Property(n => n.Body)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(n => n.IsHtml)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.Language)
            .HasMaxLength(10)
            .HasDefaultValue("en-US");

        builder.Property(n => n.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(n => n.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue("Normal");

        builder.Property(n => n.FromAddress)
            .HasMaxLength(256);

        builder.Property(n => n.FromName)
            .HasMaxLength(200);

        builder.Property(n => n.ReplyToAddress)
            .HasMaxLength(256);

        builder.Property(n => n.BccAddresses)
            .HasMaxLength(1000);

        builder.Property(n => n.AttachmentPaths)
            .HasMaxLength(2000);

        builder.Property(n => n.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(n => n.LastModifiedAt);

        builder.Property(n => n.LastUsedAt);

        builder.Property(n => n.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(n => n.IsSystemTemplate)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Complex type for notification settings
        builder.ComplexProperty(n => n.NotificationSettings, ns =>
        {
            ns.Property(s => s.EnableEmail)
              .IsRequired()
              .HasDefaultValue(true);
            
            ns.Property(s => s.EnableSms)
              .IsRequired()
              .HasDefaultValue(false);
            
            ns.Property(s => s.EnablePush)
              .IsRequired()
              .HasDefaultValue(false);
            
            ns.Property(s => s.EnableInApp)
              .IsRequired()
              .HasDefaultValue(true);
            
            ns.Property(s => s.DeliveryDelay)
              .IsRequired()
              .HasDefaultValue(0);
            
            ns.Property(s => s.RetryAttempts)
              .IsRequired()
              .HasDefaultValue(3);
            
            ns.Property(s => s.RetryInterval)
              .IsRequired()
              .HasDefaultValue(300);
            
            ns.Property(s => s.ExpiryHours)
              .IsRequired()
              .HasDefaultValue(24);
        });

        // Configure variables as JSON
        builder.Property(n => n.Variables)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure triggers as JSON
        builder.Property(n => n.Triggers)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure conditions as JSON
        builder.Property(n => n.Conditions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Configure metadata as JSON
        builder.Property(n => n.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(n => n.Name)
            .IsUnique()
            .HasDatabaseName("IX_NotificationTemplates_Name")
            .HasFillFactor(90);

        builder.HasIndex(n => n.Type)
            .HasDatabaseName("IX_NotificationTemplates_Type")
            .HasFillFactor(85);

        builder.HasIndex(n => n.Category)
            .HasDatabaseName("IX_NotificationTemplates_Category")
            .HasFillFactor(85);

        builder.HasIndex(n => n.IsEnabled)
            .HasDatabaseName("IX_NotificationTemplates_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(n => new { n.Type, n.IsEnabled })
            .HasDatabaseName("IX_NotificationTemplates_Type_IsEnabled")
            .HasFillFactor(85);

        builder.HasIndex(n => n.Language)
            .HasDatabaseName("IX_NotificationTemplates_Language")
            .HasFillFactor(85);

        builder.HasIndex(n => n.IsSystemTemplate)
            .HasDatabaseName("IX_NotificationTemplates_IsSystemTemplate")
            .HasFillFactor(85);
    }
}
