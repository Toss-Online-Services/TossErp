using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.PaymentAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseHiLo("paymentseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(p => p.SaleId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Method)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Reference)
            .HasMaxLength(100);

        builder.Property(p => p.CardLast4)
            .HasMaxLength(4);

        builder.Property(p => p.CardType)
            .HasMaxLength(50);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.PartialRefundAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.TransactionId)
            .HasMaxLength(100);

        builder.Property(p => p.AuthorizationCode)
            .HasMaxLength(100);

        builder.Property(p => p.ErrorMessage)
            .HasMaxLength(500);

        builder.Property(p => p.RetryCount)
            .IsRequired();

        builder.Property(p => p.LastRetryAt);

        builder.Property(p => p.IsReconciled)
            .IsRequired();

        builder.Property(p => p.ReconciledAt);

        builder.Property(p => p.ReconciliationReference)
            .HasMaxLength(100);

        // Configure value objects
        builder.OwnsOne(p => p.GatewayResponse, response =>
        {
            response.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);
            response.Property(r => r.TransactionId)
                .HasMaxLength(100);
            response.Property(r => r.AuthorizationCode)
                .HasMaxLength(100);
            response.Property(r => r.ErrorCode)
                .HasMaxLength(50);
            response.Property(r => r.ErrorMessage)
                .HasMaxLength(500);
            response.Property(r => r.RawResponse)
                .HasMaxLength(2000);
            response.Property(r => r.ResponseTime)
                .IsRequired();
        });

        // Configure collections
        builder.HasMany(p => p.Events)
            .WithOne()
            .HasForeignKey("PaymentId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PaymentSplits)
            .WithOne()
            .HasForeignKey("PaymentId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Sale relationship
        builder.HasOne<SaleAggregate.Sale>()
            .WithMany()
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.TransactionId)
            .IsUnique();

        builder.HasIndex(p => p.CreatedAt);
    }
} 
