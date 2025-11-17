using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.GroupBuying;

namespace Toss.Infrastructure.Data.Configurations;

public class GroupBuyPoolConfiguration : IEntityTypeConfiguration<GroupBuyPool>
{
    public void Configure(EntityTypeBuilder<GroupBuyPool> builder)
    {
        builder.Property(p => p.PoolNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.BulkDiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(p => p.FinalUnitPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.EstimatedShippingCost)
            .HasPrecision(18, 2);

        builder.Property(p => p.ActualShippingCost)
            .HasPrecision(18, 2);

        builder.Property(p => p.AreaGroup)
            .HasMaxLength(200);

        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.InitiatorShop)
            .WithMany()
            .HasForeignKey(p => p.InitiatorShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Vendor)
            .WithMany()
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Participations)
            .WithOne(pp => pp.GroupBuyPool)
            .HasForeignKey(pp => pp.GroupBuyPoolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.AggregatedPurchaseOrder)
            .WithOne(apo => apo.GroupBuyPool)
            .HasForeignKey<GroupBuyPool>(p => p.AggregatedPurchaseOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.PoolNumber)
            .IsUnique();

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => new { p.Status, p.CloseDate });
        builder.HasIndex(p => p.AreaGroup);
    }
}

