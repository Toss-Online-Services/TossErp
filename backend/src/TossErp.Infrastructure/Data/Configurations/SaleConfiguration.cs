using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Sales;

namespace TossErp.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();
        
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.CustomerName)
            .HasMaxLength(200);
        
        builder.Property(s => s.CustomerPhone)
            .HasMaxLength(20);
        
        builder.Property(s => s.CustomerEmail)
            .HasMaxLength(100);
        
        builder.Property(s => s.Subtotal)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalAmount)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.WarehouseName)
            .HasMaxLength(200);
        
        builder.Property(s => s.CashierName)
            .HasMaxLength(200);
        
        builder.Property(s => s.PosDeviceId)
            .HasMaxLength(100);
        
        builder.Property(s => s.PosDeviceName)
            .HasMaxLength(200);
        
        builder.Property(s => s.ReceiptNumber)
            .HasMaxLength(50);
        
        builder.Property(s => s.Notes)
            .HasMaxLength(1000);
        
        builder.HasIndex(s => s.SaleDate);
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.Status);
        
        // Relationships
        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(i => i.ProductSku)
            .HasMaxLength(50);
        
        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.Discount)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.LineTotal)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.Notes)
            .HasMaxLength(500);
        
        builder.HasIndex(i => i.SaleId);
        builder.HasIndex(i => i.ProductId);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.SaleNumber)
            .HasMaxLength(50);
        
        builder.Property(p => p.Method)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);
        
        builder.Property(p => p.TransactionId)
            .HasMaxLength(100);
        
        builder.Property(p => p.CardLast4)
            .HasMaxLength(4);
        
        builder.Property(p => p.CardType)
            .HasMaxLength(50);
        
        builder.Property(p => p.MobileMoneyProvider)
            .HasMaxLength(100);
        
        builder.Property(p => p.MobileMoneyNumber)
            .HasMaxLength(20);
        
        builder.Property(p => p.BankName)
            .HasMaxLength(200);
        
        builder.Property(p => p.BankAccountNumber)
            .HasMaxLength(50);
        
        builder.Property(p => p.ProcessedByName)
            .HasMaxLength(200);
        
        builder.Property(p => p.Notes)
            .HasMaxLength(500);
        
        builder.HasIndex(p => p.SaleId);
        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.Status);
    }
}

