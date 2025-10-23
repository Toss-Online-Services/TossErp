using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.CRM;

namespace Toss.Infrastructure.Data.Configurations;

public class CustomerPurchaseConfiguration : IEntityTypeConfiguration<CustomerPurchase>
{
    public void Configure(EntityTypeBuilder<CustomerPurchase> builder)
    {
        builder.Property(cp => cp.PurchaseAmount)
            .HasPrecision(18, 2);

        builder.Property(cp => cp.TopProductCategory)
            .HasMaxLength(200);

        builder.HasOne(cp => cp.Sale)
            .WithMany()
            .HasForeignKey(cp => cp.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cp => cp.PurchaseDate);
        builder.HasIndex(cp => new { cp.CustomerId, cp.PurchaseDate });
    }
}

