using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Businesses;

namespace Toss.Infrastructure.Data.Configurations.Businesses;

public class BusinessSettingsConfiguration : IEntityTypeConfiguration<BusinessSettings>
{
    public void Configure(EntityTypeBuilder<BusinessSettings> builder)
    {
        builder.Property(s => s.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(s => s.CurrencySymbol)
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(s => s.VatRate)
            .HasPrecision(5, 4); // e.g., 0.1500 for 15%

        builder.Property(s => s.DateFormat)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.TimeFormat)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Locale)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.Timezone)
            .HasMaxLength(100)
            .IsRequired();

        // Relationships
        builder.HasOne(s => s.Business)
            .WithOne(b => b.Settings)
            .HasForeignKey<BusinessSettings>(s => s.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: one settings per business
        builder.HasIndex(s => s.BusinessId)
            .IsUnique();
    }
}

