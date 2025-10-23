using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Payments;

namespace Toss.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.PaymentReference)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(p => p.SourceType)
            .HasMaxLength(50);

        builder.Property(p => p.PayerName)
            .HasMaxLength(200);

        builder.Property(p => p.PayerPhone)
            .HasMaxLength(20);

        builder.Property(p => p.PayerEmail)
            .HasMaxLength(256);

        builder.Property(p => p.GatewayReference)
            .HasMaxLength(200);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.HasOne(p => p.Shop)
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.PayLink)
            .WithMany(pl => pl.Payments)
            .HasForeignKey(p => p.PayLinkId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.PaymentReference)
            .IsUnique();

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.InitiatedAt);
        builder.HasIndex(p => new { p.SourceType, p.SourceId });
    }
}

