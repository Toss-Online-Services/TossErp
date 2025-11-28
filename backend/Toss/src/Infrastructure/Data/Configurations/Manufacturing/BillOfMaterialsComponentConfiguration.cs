using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Manufacturing;

namespace Toss.Infrastructure.Data.Configurations.Manufacturing;

public class BillOfMaterialsComponentConfiguration : IEntityTypeConfiguration<BillOfMaterialsComponent>
{
    public void Configure(EntityTypeBuilder<BillOfMaterialsComponent> builder)
    {
        builder.Property(c => c.Unit)
            .HasMaxLength(50);

        builder.Property(c => c.QuantityPer)
            .HasPrecision(18, 4);

        builder.Property(c => c.ScrapPercent)
            .HasPrecision(5, 2);

        // Relationships
        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Bom)
            .WithMany(b => b.Components)
            .HasForeignKey(c => c.BomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.ComponentProduct)
            .WithMany()
            .HasForeignKey(c => c.ComponentProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(c => c.BusinessId);
        builder.HasIndex(c => new { c.BusinessId, c.BomId });
        builder.HasIndex(c => c.ComponentProductId);
    }
}

