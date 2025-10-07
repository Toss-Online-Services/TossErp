using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Inventory;
using System.Text.Json;

namespace TossErp.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Sku)
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasFilter("\"Sku\" IS NOT NULL");
        
        builder.Property(p => p.Barcode)
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.Barcode)
            .HasFilter("\"Barcode\" IS NOT NULL");
        
        builder.Property(p => p.Description)
            .HasMaxLength(2000);
        
        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(p => p.CategoryName)
            .HasMaxLength(100);
        
        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SellingPrice)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.WholesalePrice)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Weight)
            .HasPrecision(18, 3);
        
        builder.Property(p => p.WeightUnit)
            .HasMaxLength(20);
        
        builder.Property(p => p.Dimensions)
            .HasMaxLength(100);
        
        builder.Property(p => p.TaxRate)
            .HasPrecision(5, 4);
        
        builder.Property(p => p.TaxCategory)
            .HasMaxLength(50);
        
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);
        
        // Store AdditionalImages as JSON
        builder.Property(p => p.AdditionalImages)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
            )
            .HasColumnType("jsonb");
        
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsActive);
        
        // Relationships
        builder.HasMany(p => p.StockLevels)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.StockMovements)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.ToTable("StockLevels");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.WarehouseName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(s => new { s.ProductId, s.WarehouseId })
            .IsUnique();
        
        builder.HasIndex(s => s.WarehouseId);
        
        // Computed column for available quantity (not stored, calculated on query)
        builder.Ignore(s => s.QuantityAvailable);
    }
}

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.FromWarehouseName)
            .HasMaxLength(200);
        
        builder.Property(s => s.ToWarehouseName)
            .HasMaxLength(200);
        
        builder.Property(s => s.MovementType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.ReferenceType)
            .HasMaxLength(50);
        
        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(50);
        
        builder.Property(s => s.Notes)
            .HasMaxLength(1000);
        
        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.MovementDate);
        builder.HasIndex(s => new { s.ReferenceType, s.ReferenceId });
    }
}

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(w => w.Code)
            .HasMaxLength(50);
        
        builder.HasIndex(w => w.Code)
            .IsUnique()
            .HasFilter("\"Code\" IS NOT NULL");
        
        builder.Property(w => w.Description)
            .HasMaxLength(1000);
        
        builder.Property(w => w.AddressLine1)
            .HasMaxLength(200);
        
        builder.Property(w => w.AddressLine2)
            .HasMaxLength(200);
        
        builder.Property(w => w.City)
            .HasMaxLength(100);
        
        builder.Property(w => w.Province)
            .HasMaxLength(100);
        
        builder.Property(w => w.PostalCode)
            .HasMaxLength(20);
        
        builder.Property(w => w.Country)
            .HasMaxLength(100);
        
        builder.Property(w => w.ContactPerson)
            .HasMaxLength(200);
        
        builder.Property(w => w.Phone)
            .HasMaxLength(20);
        
        builder.Property(w => w.Email)
            .HasMaxLength(100);
        
        builder.Property(w => w.WarehouseType)
            .HasMaxLength(50);
        
        builder.HasIndex(w => w.IsActive);
        builder.HasIndex(w => w.IsDefault);
        
        // Relationships
        builder.HasMany(w => w.StockLevels)
            .WithOne()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

