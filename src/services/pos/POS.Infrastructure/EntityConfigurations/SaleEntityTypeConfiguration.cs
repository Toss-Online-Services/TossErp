using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class SaleEntityTypeConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales", "POS");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.StoreId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.StaffId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.PaymentMethodId)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.BuyerId)
            .HasConversion<string>();

        builder.Property(s => s.CardTypeId)
            .HasConversion<string>();

        builder.Property(s => s.CardNumber)
            .HasMaxLength(50);

        builder.Property(s => s.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.SaleDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.AmountPaid)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.IsSynced)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.InvoiceNumber);
        builder.HasIndex(s => s.SaleDate);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StaffId);
        builder.HasIndex(s => s.BuyerId);
        builder.HasIndex(s => s.IsSynced);

        // Relationships
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Payments)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Discounts)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Buyer)
            .WithMany()
            .HasForeignKey(s => s.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Staff)
            .WithMany()
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.PaymentMethod)
            .WithMany()
            .HasForeignKey(s => s.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.CardType)
            .WithMany()
            .HasForeignKey(s => s.CardTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
