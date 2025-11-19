using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Payments;

namespace Toss.Infrastructure.Data.Configurations;

public class PayLinkConfiguration : IEntityTypeConfiguration<PayLink>
{
    public void Configure(EntityTypeBuilder<PayLink> builder)
    {
        builder.Property(pl => pl.LinkCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pl => pl.FullUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pl => pl.Amount)
            .HasPrecision(18, 2);

        builder.Property(pl => pl.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(pl => pl.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pl => pl.SourceType)
            .HasMaxLength(50);

        builder.Property(pl => pl.CustomerName)
            .HasMaxLength(200);

        builder.Property(pl => pl.CustomerPhone)
            .HasMaxLength(20);

        builder.Property(pl => pl.CustomerEmail)
            .HasMaxLength(256);

        builder.HasOne(pl => pl.Shop)
            .WithMany()
            .HasForeignKey(pl => pl.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pl => pl.Customer)
            .WithMany()
            .HasForeignKey(pl => pl.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(pl => pl.LinkCode)
            .IsUnique();

        builder.HasIndex(pl => new { pl.IsActive, pl.ExpiresAt });
        builder.HasIndex(pl => new { pl.SourceType, pl.SourceId });
    }
}

