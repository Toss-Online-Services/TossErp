using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.JobTitle)
            .HasMaxLength(100);

        builder.Property(u => u.Department)
            .HasMaxLength(100);

        builder.Property(u => u.EmployeeId)
            .HasMaxLength(50);

        builder.Property(u => u.ManagerId)
            .HasMaxLength(50);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.LastLoginAt);

        builder.Property(u => u.PasswordLastChangedAt);

        builder.Property(u => u.FailedLoginAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(u => u.IsLocked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.LockedUntil);

        builder.Property(u => u.TwoFactorEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.TwoFactorSecret)
            .HasMaxLength(200);

        builder.Property(u => u.PreferredLanguage)
            .HasMaxLength(10)
            .HasDefaultValue("en-US");

        builder.Property(u => u.PreferredTimezone)
            .HasMaxLength(50)
            .HasDefaultValue("UTC");

        builder.Property(u => u.PreferredDateFormat)
            .HasMaxLength(20)
            .HasDefaultValue("yyyy-MM-dd");

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        // Complex type for security policy
        builder.ComplexProperty(u => u.SecurityPolicy, sp =>
        {
            sp.Property(p => p.RequirePasswordChange)
              .IsRequired()
              .HasDefaultValue(false);
            
            sp.Property(p => p.PasswordExpiryDays)
              .IsRequired()
              .HasDefaultValue(90);
            
            sp.Property(p => p.RequireTwoFactor)
              .IsRequired()
              .HasDefaultValue(false);
            
            sp.Property(p => p.AllowedIpAddresses)
              .HasMaxLength(1000);
            
            sp.Property(p => p.SessionTimeoutMinutes)
              .IsRequired()
              .HasDefaultValue(30);
            
            sp.Property(p => p.MaxConcurrentSessions)
              .IsRequired()
              .HasDefaultValue(3);
        });

        // Configure permissions as JSON
        builder.Property(u => u.Permissions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure roles as JSON
        builder.Property(u => u.Roles)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        // Configure custom fields as JSON
        builder.Property(u => u.CustomFields)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("nvarchar(max)");

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_UserProfiles_Email")
            .HasFillFactor(90);

        builder.HasIndex(u => u.EmployeeId)
            .HasDatabaseName("IX_UserProfiles_EmployeeId")
            .HasFillFactor(85);

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_UserProfiles_IsActive")
            .HasFillFactor(85);

        builder.HasIndex(u => new { u.IsActive, u.IsLocked })
            .HasDatabaseName("IX_UserProfiles_IsActive_IsLocked")
            .HasFillFactor(85);

        builder.HasIndex(u => u.Department)
            .HasDatabaseName("IX_UserProfiles_Department")
            .HasFillFactor(85);

        builder.HasIndex(u => u.ManagerId)
            .HasDatabaseName("IX_UserProfiles_ManagerId")
            .HasFillFactor(85);
    }
}
