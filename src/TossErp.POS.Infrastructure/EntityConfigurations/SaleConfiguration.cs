using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Enums;

namespace TossErp.POS.Infrastructure.EntityConfigurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.SaleNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.CustomerId)
                .IsRequired();

            builder.Property(s => s.CashierId)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.SaleType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.SubTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.AmountPaid)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.ChangeAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.PaymentMethod)
                .HasConversion<int>();

            builder.Property(s => s.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(s => s.Notes)
                .HasMaxLength(1000);

            builder.Property(s => s.SaleDate)
                .IsRequired();

            builder.Property(s => s.CompletedAt);
            builder.Property(s => s.CancelledAt);
            builder.Property(s => s.CancellationReason)
                .HasMaxLength(500);

            builder.HasIndex(s => s.SaleNumber).IsUnique();
            builder.HasIndex(s => s.CustomerId);
            builder.HasIndex(s => s.CashierId);
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => s.SaleDate);
            builder.HasIndex(s => s.CompletedAt);

            builder.HasMany(s => s.SaleItems)
                .WithOne()
                .HasForeignKey("SaleId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Payments)
                .WithOne()
                .HasForeignKey("SaleId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 
