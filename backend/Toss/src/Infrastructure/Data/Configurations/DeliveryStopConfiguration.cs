using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Logistics;

namespace Toss.Infrastructure.Data.Configurations;

public class DeliveryStopConfiguration : IEntityTypeConfiguration<DeliveryStop>
{
    public void Configure(EntityTypeBuilder<DeliveryStop> builder)
    {
        builder.Property(s => s.CostShare)
            .HasPrecision(18, 2);

        builder.Property(s => s.DeliveryInstructions)
            .HasMaxLength(500);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.ComplexProperty(s => s.DeliveryLocation, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude).IsRequired();
            locationBuilder.Property(l => l.Longitude).IsRequired();
            locationBuilder.Property(l => l.Area).HasMaxLength(200);
            locationBuilder.Property(l => l.Zone).HasMaxLength(200);
        });

        builder.HasOne(s => s.Shop)
            .WithMany()
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ProofOfDelivery)
            .WithOne(pod => pod.DeliveryStop)
            .HasForeignKey<DeliveryStop>(s => s.ProofOfDeliveryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => new { s.SharedDeliveryRunId, s.SequenceNumber });
        builder.HasIndex(s => s.Status);
    }
}

