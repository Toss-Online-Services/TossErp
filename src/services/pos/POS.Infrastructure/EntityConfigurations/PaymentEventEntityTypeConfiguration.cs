using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.PaymentAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class PaymentEventEntityTypeConfiguration : IEntityTypeConfiguration<PaymentEvent>
{
    public void Configure(EntityTypeBuilder<PaymentEvent> builder)
    {
        builder.ToTable("PaymentEvents", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(pe => new { pe.PaymentId, pe.Type, pe.CreatedAt });

        builder.Property(pe => pe.PaymentId)
            .IsRequired();

        builder.Property(pe => pe.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pe => pe.Details)
            .HasMaxLength(500);

        builder.Property(pe => pe.CreatedAt)
            .IsRequired();

        // Configure Payment relationship
        builder.HasOne<Payment>()
            .WithMany(p => p.Events)
            .HasForeignKey(pe => pe.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pe => pe.CreatedAt);
    }
} 
