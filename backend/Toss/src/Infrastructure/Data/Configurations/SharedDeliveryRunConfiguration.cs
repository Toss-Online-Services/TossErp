using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Logistics;

namespace Toss.Infrastructure.Data.Configurations;

public class SharedDeliveryRunConfiguration : IEntityTypeConfiguration<SharedDeliveryRun>
{
    public void Configure(EntityTypeBuilder<SharedDeliveryRun> builder)
    {
        builder.Property(r => r.RunNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.TotalDeliveryCost)
            .HasPrecision(18, 2);

        builder.Property(r => r.CostPerStop)
            .HasPrecision(18, 2);

        builder.Property(r => r.AreaGroup)
            .HasMaxLength(200);

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.ComplexProperty(r => r.StartLocation, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude);
            locationBuilder.Property(l => l.Longitude);
            locationBuilder.Property(l => l.Area).HasMaxLength(200);
            locationBuilder.Property(l => l.Zone).HasMaxLength(200);
        });

        builder.HasOne(r => r.Driver)
            .WithMany(d => d.DeliveryRuns)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(r => r.Stops)
            .WithOne(s => s.SharedDeliveryRun)
            .HasForeignKey(s => s.SharedDeliveryRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.RunNumber)
            .IsUnique();

        builder.HasIndex(r => r.ScheduledDate);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.GroupBuyPoolId);
    }
}

