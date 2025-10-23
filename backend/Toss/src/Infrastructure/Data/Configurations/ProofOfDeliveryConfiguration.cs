using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Logistics;

namespace Toss.Infrastructure.Data.Configurations;

public class ProofOfDeliveryConfiguration : IEntityTypeConfiguration<ProofOfDelivery>
{
    public void Configure(EntityTypeBuilder<ProofOfDelivery> builder)
    {
        builder.Property(pod => pod.PIN)
            .HasMaxLength(10);

        builder.Property(pod => pod.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(pod => pod.SignedBy)
            .HasMaxLength(200);

        builder.Property(pod => pod.Notes)
            .HasMaxLength(1000);

        builder.ComplexProperty(pod => pod.CaptureLocation, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude);
            locationBuilder.Property(l => l.Longitude);
            locationBuilder.Property(l => l.Area).HasMaxLength(200);
            locationBuilder.Property(l => l.Zone).HasMaxLength(200);
        });

        builder.HasIndex(pod => pod.CapturedAt);
    }
}

