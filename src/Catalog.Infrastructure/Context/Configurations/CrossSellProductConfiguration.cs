using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Context.Configurations;

public class CrossSellProductConfiguration : IEntityTypeConfiguration<CrossSellProduct>
{
    public void Configure(EntityTypeBuilder<CrossSellProduct> builder)
    {
        builder.ToTable("CrossSellProducts");

        builder.HasKey(csp => csp.Id);

        builder.HasOne(csp => csp.Product1)
            .WithMany(p => p.CrossSellProducts)
            .HasForeignKey(csp => csp.ProductId1)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(csp => csp.Product2)
            .WithMany()
            .HasForeignKey(csp => csp.ProductId2)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
