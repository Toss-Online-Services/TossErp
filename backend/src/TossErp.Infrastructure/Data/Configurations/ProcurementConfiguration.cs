using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Procurement;

namespace TossErp.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(s => s.Email)
            .HasMaxLength(100);
        
        builder.Property(s => s.Phone)
            .HasMaxLength(20);
        
        builder.Property(s => s.Website)
            .HasMaxLength(200);
        
        builder.Property(s => s.TaxNumber)
            .HasMaxLength(50);
        
        builder.Property(s => s.RegistrationNumber)
            .HasMaxLength(50);
        
        builder.Property(s => s.CreditLimit)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.CurrentBalance)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.AverageLeadTimeDays)
            .HasPrecision(10, 2);
        
        builder.Property(s => s.QualityRating)
            .HasPrecision(3, 2);
        
        builder.Property(s => s.DeliveryRating)
            .HasPrecision(3, 2);
        
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.Email);
        
        // Relationships
        builder.HasMany(s => s.PurchaseOrders)
            .WithOne(p => p.Supplier)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.OrderNumber)
            .IsUnique();
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.Subtotal)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.ShippingCost)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.WarehouseName)
            .HasMaxLength(200);
        
        builder.Property(p => p.Notes)
            .HasMaxLength(1000);
        
        builder.Property(p => p.Terms)
            .HasMaxLength(1000);
        
        builder.Property(p => p.ApprovedByName)
            .HasMaxLength(200);
        
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.OrderDate);
        builder.HasIndex(p => p.SupplierId);
        
        // Relationships
        builder.HasMany(p => p.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(i => i.ProductSku)
            .HasMaxLength(50);
        
        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.LineTotal)
            .HasPrecision(18, 2);
        
        builder.Property(i => i.Notes)
            .HasMaxLength(500);
        
        builder.HasIndex(i => i.PurchaseOrderId);
        builder.HasIndex(i => i.ProductId);
    }
}

