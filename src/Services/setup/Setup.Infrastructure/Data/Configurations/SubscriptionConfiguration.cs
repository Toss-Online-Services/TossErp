using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Setup.Domain.Aggregates.TenantAggregate;

namespace Setup.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.TenantId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.PlanId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.TrialEndDate);

        builder.Property(s => s.CancellationDate);

        builder.Property(s => s.PaymentMethodId)
            .HasMaxLength(100);

        builder.Property(s => s.CustomerId)
            .HasMaxLength(100);

        builder.Property(s => s.IsAutoRenew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        // Indexes with EF Core 9 fill factors
        builder.HasIndex(s => s.TenantId)
            .IsUnique()
            .HasDatabaseName("IX_Subscriptions_TenantId")
            .HasFillFactor(90);

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Subscriptions_Status")
            .HasFillFactor(85);

        builder.HasIndex(s => new { s.Status, s.EndDate })
            .HasDatabaseName("IX_Subscriptions_Status_EndDate")
            .HasFillFactor(85);

        builder.HasIndex(s => s.CustomerId)
            .HasDatabaseName("IX_Subscriptions_CustomerId")
            .HasFillFactor(85);
    }
}
