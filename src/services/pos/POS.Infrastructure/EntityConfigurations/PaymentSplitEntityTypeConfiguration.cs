using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.PaymentAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class PaymentSplitEntityTypeConfiguration : IEntityTypeConfiguration<PaymentSplit>
{
    public void Configure(EntityTypeBuilder<PaymentSplit> builder)
    {
        builder.ToTable("PaymentSplits", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(ps => new { ps.PaymentId, ps.Amount, ps.Method, ps.CreatedAt });

        builder.Property(ps => ps.PaymentId)
            .IsRequired();

        builder.Property(ps => ps.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ps => ps.Method)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ps => ps.Reference)
            .HasMaxLength(100);

        builder.Property(ps => ps.CardLast4)
            .HasMaxLength(4);

        builder.Property(ps => ps.CardType)
            .HasMaxLength(50);

        builder.Property(ps => ps.CreatedAt)
            .IsRequired();

        // Configure Payment relationship
        builder.HasOne<Payment>()
            .WithMany(p => p.PaymentSplits)
            .HasForeignKey(ps => ps.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ps => ps.CreatedAt);
    }
} 
