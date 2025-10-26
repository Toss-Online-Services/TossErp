using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.Property(po => po.PONumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(po => po.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(po => po.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(po => po.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(po => po.Total)
            .HasPrecision(18, 2);

        builder.Property(po => po.Notes)
            .HasMaxLength(1000);

        builder.HasOne(po => po.Shop)
            .WithMany()
            .HasForeignKey(po => po.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(po => po.Vendor)
            .WithMany()
            .HasForeignKey(po => po.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.Receipts)
            .WithOne(r => r.PurchaseOrder)
            .HasForeignKey(r => r.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(po => po.PONumber)
            .IsUnique();

        builder.HasIndex(po => po.OrderDate);
        builder.HasIndex(po => new { po.ShopId, po.OrderDate });
        builder.HasIndex(po => po.Status);
        builder.HasIndex(po => po.GroupBuyPoolId);
    }
}

