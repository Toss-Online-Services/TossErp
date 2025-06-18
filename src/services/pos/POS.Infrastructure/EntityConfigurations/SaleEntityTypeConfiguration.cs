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

        builder.Property(s => s.StoreId)
            .IsRequired();

        builder.Property(s => s.CustomerId);

        builder.Property(s => s.StaffId);

        builder.Property(s => s.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Tax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Discount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.CompletedAt);

        builder.Property(s => s.CancelledAt);

        builder.Property(s => s.CancellationReason)
            .HasMaxLength(500);

        builder.Property(s => s.RequiresSync)
            .IsRequired();

        builder.Property(s => s.LastSyncedAt);

        builder.Property(s => s.SyncError)
            .HasMaxLength(500);

        // Configure Address value object
        builder.OwnsOne(s => s.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200);
            a.Property(addr => addr.City).HasMaxLength(100);
            a.Property(addr => addr.State).HasMaxLength(100);
            a.Property(addr => addr.Country).HasMaxLength(100);
            a.Property(addr => addr.PostalCode).HasMaxLength(20);
        });

        // Configure collections
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

        // Configure indexes
        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.HasIndex(s => s.CreatedAt);

        builder.HasIndex(s => s.StoreId);

        builder.HasIndex(s => s.CustomerId);

        builder.HasIndex(s => s.StaffId);

        builder.HasIndex(s => s.Status);

        builder.HasIndex(s => s.RequiresSync);

        builder.HasIndex(s => s.UpdatedAt);

        builder.HasIndex(s => s.CompletedAt);

        builder.HasIndex(s => s.CancelledAt);

        builder.HasIndex(s => s.Total);
    }
} 
