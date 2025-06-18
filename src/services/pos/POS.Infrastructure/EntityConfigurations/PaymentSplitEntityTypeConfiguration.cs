using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.PaymentAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class PaymentSplitEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.OwnsMany(p => p.PaymentSplits, ps =>
        {
            ps.Property(split => split.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            ps.Property(split => split.Method)
                .IsRequired()
                .HasMaxLength(50);

            ps.Property(split => split.Reference)
                .HasMaxLength(100);

            ps.Property(split => split.CardLast4)
                .HasMaxLength(4);

            ps.Property(split => split.CardType)
                .HasMaxLength(50);

            ps.Property(split => split.CreatedAt)
                .IsRequired();

            ps.HasIndex(split => split.CreatedAt);
        });
    }
} 
