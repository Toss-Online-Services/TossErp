using POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", "POS");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion<string>().IsRequired();

        builder.Property(p => p.SaleId).HasConversion<string>().IsRequired();
        builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Reference).HasMaxLength(100);
        builder.Property(p => p.Type).HasConversion<string>().IsRequired();
        builder.Property(p => p.CardLast4).HasMaxLength(4);
        builder.Property(p => p.CardType).HasMaxLength(50);
        builder.Property(p => p.TransactionId).HasMaxLength(100);
        builder.Property(p => p.Status).HasMaxLength(20).IsRequired();
        builder.Property(p => p.ErrorMessage).HasMaxLength(500);
        builder.Property(p => p.PaymentDate).IsRequired();
        builder.Property(p => p.ProcessedAt);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);

        builder.HasIndex(p => p.SaleId);
        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.TransactionId);
        builder.HasIndex(p => p.Type);

        builder.HasOne(p => p.Sale)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
