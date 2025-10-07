using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.SupplyChain;

namespace TossErp.Infrastructure.Data.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(s => s.ShipmentNumber)
            .IsUnique();
        
        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.ShippingCost)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.InsuranceCost)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.OtherCharges)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalCost)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalWeight)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalVolume)
            .HasPrecision(18, 4);
        
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.TrackingNumber);
        builder.HasIndex(s => s.ExpectedDeliveryDate);
        
        // Relationships
        builder.HasMany(s => s.Items)
            .WithOne(i => i.Shipment)
            .HasForeignKey(i => i.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.TrackingHistory)
            .WithOne(t => t.Shipment)
            .HasForeignKey(t => t.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.ToTable("ShipmentItems");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(i => i.QuantityOrdered)
            .HasPrecision(18, 4);
        
        builder.Property(i => i.QuantityShipped)
            .HasPrecision(18, 4);
        
        builder.Property(i => i.QuantityReceived)
            .HasPrecision(18, 4);
        
        builder.Property(i => i.QuantityDamaged)
            .HasPrecision(18, 4);
        
        builder.Property(i => i.Weight)
            .HasPrecision(18, 2);
        
        builder.HasIndex(i => i.ShipmentId);
        builder.HasIndex(i => i.ProductId);
    }
}

public class ShipmentTrackingConfiguration : IEntityTypeConfiguration<ShipmentTracking>
{
    public void Configure(EntityTypeBuilder<ShipmentTracking> builder)
    {
        builder.ToTable("ShipmentTrackingHistory");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(t => t.Latitude)
            .HasPrecision(10, 7);
        
        builder.Property(t => t.Longitude)
            .HasPrecision(10, 7);
        
        builder.HasIndex(t => t.ShipmentId);
        builder.HasIndex(t => t.EventDate);
    }
}

public class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
{
    public void Configure(EntityTypeBuilder<Carrier> builder)
    {
        builder.ToTable("Carriers");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.Code)
            .HasMaxLength(50);
        
        builder.HasIndex(c => c.Code)
            .HasFilter("\"Code\" IS NOT NULL");
        
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.MinimumShipmentValue)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.MaximumWeight)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.OnTimeDeliveryRate)
            .HasPrecision(5, 2);
        
        builder.Property(c => c.DamageRate)
            .HasPrecision(5, 2);
        
        builder.Property(c => c.AverageRating)
            .HasPrecision(3, 2);
        
        builder.HasIndex(c => c.IsActive);
    }
}
