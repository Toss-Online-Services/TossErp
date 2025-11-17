using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Tax;

namespace Toss.Infrastructure.Data.Configurations;

public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Percentage)
            .HasPrecision(18, 4);

        builder.Property(x => x.Zip)
            .HasMaxLength(30);

        builder.HasOne(x => x.TaxCategory)
            .WithMany()
            .HasForeignKey(x => x.TaxCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

