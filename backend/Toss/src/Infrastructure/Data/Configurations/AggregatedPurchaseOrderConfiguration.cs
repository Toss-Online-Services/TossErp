using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.GroupBuying;

namespace Toss.Infrastructure.Data.Configurations;

public class AggregatedPurchaseOrderConfiguration : IEntityTypeConfiguration<AggregatedPurchaseOrder>
{
    public void Configure(EntityTypeBuilder<AggregatedPurchaseOrder> builder)
    {
        builder.Property(apo => apo.APONumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(apo => apo.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(apo => apo.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(apo => apo.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(apo => apo.Total)
            .HasPrecision(18, 2);

        builder.Property(apo => apo.Notes)
            .HasMaxLength(1000);

        builder.HasOne(apo => apo.Supplier)
            .WithMany()
            .HasForeignKey(apo => apo.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(apo => apo.APONumber)
            .IsUnique();

        builder.HasIndex(apo => apo.OrderDate);
        builder.HasIndex(apo => apo.Status);
    }
}

