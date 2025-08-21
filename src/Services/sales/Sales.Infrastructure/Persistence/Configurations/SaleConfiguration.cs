using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Sale entity
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales", "sales");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        // Multi-tenancy
        builder.Property<string>("TenantId")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex("TenantId", "ReceiptNumber")
            .HasDatabaseName("IX_Sales_TenantId_ReceiptNumber")
            .IsUnique();

        // Receipt number as value object
        builder.OwnsOne(x => x.ReceiptNumber, rn =>
        {
            rn.Property(x => x.Value)
                .HasColumnName("ReceiptNumber")
                .HasMaxLength(20)
                .IsRequired();

            rn.HasIndex(x => x.Value)
                .HasDatabaseName("IX_Sales_ReceiptNumber")
                .IsUnique();
        });

        // Basic properties
        builder.Property(x => x.TillId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired(false);

        builder.Property(x => x.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Money value objects
        builder.OwnsOne(x => x.SubTotal, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("SubTotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("SubTotalCurrency")
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

        builder.OwnsOne(x => x.DiscountAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("DiscountAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        builder.OwnsOne(x => x.Total, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        builder.OwnsOne(x => x.PaidAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("PaidAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("PaidCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        builder.OwnsOne(x => x.ChangeAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("ChangeAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("ChangeCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("ZAR")
                .IsRequired();
        });

        // Additional properties
        builder.Property(x => x.Notes)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.DiscountReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        builder.Property(x => x.CancelledAt)
            .IsRequired(false);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500)
            .IsRequired(false);

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

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Payments)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.TillId)
            .HasDatabaseName("IX_Sales_TillId");

        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_Sales_CustomerId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Sales_Status");

        builder.HasIndex("CreatedDate")
            .HasDatabaseName("IX_Sales_CreatedDate");

        builder.HasIndex(x => x.CompletedAt)
            .HasDatabaseName("IX_Sales_CompletedAt");

        // Ignore domain events (not persisted)
        builder.Ignore(x => x.DomainEvents);
    }
}
