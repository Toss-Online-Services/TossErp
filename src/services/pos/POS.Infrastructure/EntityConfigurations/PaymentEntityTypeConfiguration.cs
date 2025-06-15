using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", "POS");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion<string>();

        builder.Property(p => p.SaleId).HasConversion<string>().IsRequired();
        builder.Property(p => p.PaymentMethodId).HasConversion<string>().IsRequired();
        builder.Property(p => p.CardTypeId).HasConversion<string>();

        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.CardNumber).HasMaxLength(50);
        builder.Property(p => p.TransactionId).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.Property(p => p.PaymentDate).IsRequired();

        builder.HasIndex(p => p.SaleId);
        builder.HasIndex(p => p.PaymentMethodId);
        builder.HasIndex(p => p.CardTypeId);
        builder.HasIndex(p => p.PaymentDate);

        builder.HasOne(p => p.PaymentMethod)
            .WithMany()
            .HasForeignKey(p => p.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CardType)
            .WithMany()
            .HasForeignKey(p => p.CardTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
