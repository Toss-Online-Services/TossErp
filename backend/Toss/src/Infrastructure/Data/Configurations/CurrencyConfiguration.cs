using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Directory;

namespace Toss.Infrastructure.Data.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(x => x.Rate)
            .HasPrecision(18, 8);

        builder.Property(x => x.DisplayLocale)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CustomFormatting)
            .HasMaxLength(50);
    }
}

