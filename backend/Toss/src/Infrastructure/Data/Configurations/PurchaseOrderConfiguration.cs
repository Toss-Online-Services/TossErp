using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PurchaseOrder entity.
/// Configures vendor purchase orders with proper relationships and status tracking.
/// </summary>
public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        // Required unique PO number
        builder.Property(po => po.PONumber)
            .HasMaxLength(50)
            .IsRequired();

        // Monetary values with 2 decimal places
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

        // Relationships
        builder.HasOne(po => po.Shop)
            .WithMany()
            .HasForeignKey(po => po.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete POs if shop is deleted

        builder.HasOne(po => po.Vendor)
            .WithMany()
            .HasForeignKey(po => po.VendorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent vendor deletion if POs exist

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade); // Delete items with PO

        builder.HasMany(po => po.Receipts)
            .WithOne(r => r.PurchaseOrder)
            .HasForeignKey(r => r.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade); // Delete receipts with PO

        // Indexes for performance
        builder.HasIndex(po => po.PONumber)
            .IsUnique(); // Enforce unique PO numbers

        builder.HasIndex(po => po.OrderDate); // For date range queries
        builder.HasIndex(po => new { po.ShopId, po.OrderDate }); // Composite for shop PO reports
        builder.HasIndex(po => po.Status); // For filtering by status
        builder.HasIndex(po => po.GroupBuyPoolId); // For group buying queries
    }
}

