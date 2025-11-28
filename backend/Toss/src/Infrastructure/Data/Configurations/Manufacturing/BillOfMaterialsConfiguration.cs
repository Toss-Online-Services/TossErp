using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Manufacturing;

namespace Toss.Infrastructure.Data.Configurations.Manufacturing;

public class BillOfMaterialsConfiguration : IEntityTypeConfiguration<BillOfMaterials>
{
    public void Configure(EntityTypeBuilder<BillOfMaterials> builder)
    {
        builder.Property(b => b.Version)
            .HasMaxLength(50);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(b => b.Business)
            .WithMany()
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Components)
            .WithOne(c => c.Bom)
            .HasForeignKey(c => c.BomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(b => b.BusinessId);
        builder.HasIndex(b => new { b.BusinessId, b.ProductId });
        
        // Unique constraint: only one active BOM per product per business
        builder.HasIndex(b => new { b.BusinessId, b.ProductId, b.IsActive })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}

