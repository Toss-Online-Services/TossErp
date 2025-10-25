using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Shipping;

namespace Toss.Infrastructure.Data.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TrackingNumber)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.TotalWeight)
            .HasPrecision(18, 4);

        builder.HasMany(x => x.ShipmentItems)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

