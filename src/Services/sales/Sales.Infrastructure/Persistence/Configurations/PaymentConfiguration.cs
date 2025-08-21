using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "sales");

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

        // Payment properties
        builder.Property(x => x.Method)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.ProcessedAt)
            .IsRequired();

        // Money value object
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("Currency")
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
            .HasDatabaseName("IX_Payments_SaleId");

        builder.HasIndex(x => x.Method)
            .HasDatabaseName("IX_Payments_Method");

        builder.HasIndex(x => x.ProcessedAt)
            .HasDatabaseName("IX_Payments_ProcessedAt");

        builder.HasIndex("TenantId")
            .HasDatabaseName("IX_Payments_TenantId");
    }
}
