using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Orders;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the PurchaseRequestLine entity.
/// </summary>
public class PurchaseRequestLineConfiguration : IEntityTypeConfiguration<PurchaseRequestLine>
{
    public void Configure(EntityTypeBuilder<PurchaseRequestLine> builder)
    {
        builder.Property(prl => prl.QuantityRequested)
            .HasPrecision(18, 3) // Allow fractional quantities (e.g., 2.5 kg)
            .IsRequired();

        builder.Property(prl => prl.Remarks)
            .HasMaxLength(500);

        builder.HasOne(prl => prl.Item)
            .WithMany()
            .HasForeignKey(prl => prl.ItemId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if PR lines exist

        // Indexes for performance
        builder.HasIndex(prl => prl.PurchaseRequestId); // For PR queries
        builder.HasIndex(prl => prl.ItemId); // For product queries
    }
}

