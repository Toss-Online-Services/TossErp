using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for SaleItem entity
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems", "sales");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        // Multi-tenancy
        builder.Property<string>("TenantId")
            .HasMaxLength(50)
            .IsRequired();

        // Foreign key to Sale
        builder.Property<Guid>("SaleId")
            .IsRequired();

        // Item properties
        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ItemSku)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(x => x.TaxRate)
            .HasColumnType("decimal(5,4)")
            .IsRequired();

        // Money value objects
        builder.OwnsOne(x => x.UnitPrice, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        builder.OwnsOne(x => x.LineTotal, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("LineTotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("LineTotalCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        builder.OwnsOne(x => x.TaxAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("TaxAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        // Audit fields
        builder.Property<DateTime>("CreatedDate")
            .IsRequired();

        builder.Property<DateTime?>("UpdatedDate")
            .IsRequired(false);

        builder.Property<string>("CreatedBy")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property<string>("UpdatedBy")
            .HasMaxLength(100)
            .IsRequired(false);

        // Indexes
        builder.HasIndex("SaleId")
            .HasDatabaseName("IX_SaleItems_SaleId");

        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_SaleItems_ItemId");

        builder.HasIndex(x => x.ItemSku)
            .HasDatabaseName("IX_SaleItems_ItemSku");

        builder.HasIndex("TenantId")
            .HasDatabaseName("IX_SaleItems_TenantId");
    }
}
