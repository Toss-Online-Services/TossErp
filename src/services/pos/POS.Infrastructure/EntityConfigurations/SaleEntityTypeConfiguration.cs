using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class SaleEntityTypeConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .UseHiLo("saleseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.TaxAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Store)
            .WithMany()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Payment)
            .WithOne()
            .HasForeignKey<Sale>("PaymentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.HasIndex(s => s.CreatedAt);
    }
} 
