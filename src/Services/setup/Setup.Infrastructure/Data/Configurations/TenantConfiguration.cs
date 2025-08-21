using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;
using Setup.Domain.ValueObjects;

namespace Setup.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.DatabaseName)
            .HasMaxLength(100);

        builder.Property(t => t.ConnectionString)
            .HasMaxLength(500);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Complex type for subscription plan
        builder.ComplexProperty(t => t.SubscriptionPlan, sp =>
        {
            sp.Property(p => p.PlanName)
              .HasMaxLength(100)
              .IsRequired();
            
            sp.Property(p => p.PlanType)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
            
            sp.Property(p => p.MaxUsers)
              .IsRequired();
            
            sp.Property(p => p.MaxStorage)
              .IsRequired();
            
            sp.Property(p => p.Price)
              .HasPrecision(18, 2)
              .IsRequired();
            
            sp.Property(p => p.Currency)
              .HasMaxLength(3)
              .IsRequired();
            
            sp.Property(p => p.Features)
              .HasMaxLength(2000);
        });

        // Complex type for billing cycle
        builder.ComplexProperty(t => t.BillingCycle, bc =>
        {
            bc.Property(c => c.Type)
              .HasConversion<string>()
              .HasMaxLength(20)
              .IsRequired();
            
            bc.Property(c => c.StartDate)
              .IsRequired();
            
            bc.Property(c => c.EndDate)
              .IsRequired();
            
            bc.Property(c => c.IsAutoRenew)
              .IsRequired()
              .HasDefaultValue(true);
            
            bc.Property(c => c.GracePeriodDays)
              .IsRequired()
              .HasDefaultValue(7);
        });

        // Complex type for usage quota
        builder.ComplexProperty(t => t.UsageQuota, uq =>
        {
            uq.Property(q => q.StorageUsed)
              .IsRequired()
              .HasDefaultValue(0);
            
            uq.Property(q => q.StorageLimit)
              .IsRequired();
            
            uq.Property(q => q.UsersCount)
              .IsRequired()
              .HasDefaultValue(0);
            
            uq.Property(q => q.UsersLimit)
              .IsRequired();
            
            uq.Property(q => q.ApiCallsThisMonth)
              .IsRequired()
              .HasDefaultValue(0);
            
            uq.Property(q => q.ApiCallsLimit)
              .IsRequired();
            
            uq.Property(q => q.LastResetDate)
              .IsRequired();
        });

        // Configure relationships
        builder.HasOne<Subscription>()
            .WithOne()
            .HasForeignKey<Subscription>(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<UserProfile>()
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<IntegrationConfiguration>()
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<AuditConfiguration>()
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<StorageMetrics>()
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<ComplianceFramework>()
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(t => t.Code)
            .IsUnique()
            .HasDatabaseName("IX_Tenants_Code")
            .HasFillFactor(90);

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tenants_Status")
            .HasFillFactor(85);

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_Tenants_IsActive")
            .HasFillFactor(85);

        builder.HasIndex(t => new { t.Status, t.IsActive })
            .HasDatabaseName("IX_Tenants_Status_IsActive")
            .HasFillFactor(85);
    }
}
