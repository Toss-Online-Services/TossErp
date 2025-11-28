using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PurchaseRequest entity.
/// Configures purchase requests with proper relationships and status tracking.
/// </summary>
public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        // Required unique PR number per business (enforced via Shop.BusinessId)
        builder.Property(pr => pr.PRNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pr => pr.RequestedByUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(pr => pr.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(pr => pr.Shop)
            .WithMany()
            .HasForeignKey(pr => pr.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete PRs if shop is deleted

        builder.HasOne(pr => pr.Vendor)
            .WithMany()
            .HasForeignKey(pr => pr.VendorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent vendor deletion if PRs exist

        builder.HasOne(pr => pr.PurchaseOrder)
            .WithMany()
            .HasForeignKey(pr => pr.PurchaseOrderId)
            .OnDelete(DeleteBehavior.SetNull); // Allow PO deletion, set PR.PurchaseOrderId to null

        builder.HasMany(pr => pr.Items)
            .WithOne(i => i.PurchaseRequest)
            .HasForeignKey(i => i.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Cascade); // Delete items with PR

        // Indexes for performance
        builder.HasIndex(pr => pr.PRNumber); // Note: Unique constraint per business enforced via application logic

        builder.HasIndex(pr => new { pr.ShopId, pr.PRNumber })
            .IsUnique(); // Enforce unique PR numbers per shop (which implies per business)

        builder.HasIndex(pr => pr.Status); // For filtering by status
        builder.HasIndex(pr => pr.VendorId); // For vendor queries
        builder.HasIndex(pr => pr.RequiredByDate); // For date range queries
        builder.HasIndex(pr => pr.PurchaseOrderId); // For PR→PO conversion queries
    }
}

