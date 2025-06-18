using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .UseHiLo("orderseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.TaxRate)
            .IsRequired()
            .HasPrecision(5, 4); // Allows values from 0 to 1 with 4 decimal places

        builder.Property(o => o.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5, 4); // Allows values from 0 to 1 with 4 decimal places

        // Configure Money value objects
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.OwnsOne(o => o.TaxAmount, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.OwnsOne(o => o.DiscountAmount, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CompletedAt);

        builder.Property(o => o.Notes)
            .HasMaxLength(500);

        // Configure OrderItems collection
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for performance
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.CustomerId);

        builder.HasIndex(o => o.Status);

        builder.HasIndex(o => o.CreatedAt);

        builder.HasIndex(o => o.CompletedAt);

        builder.HasIndex(o => o.UpdatedAt);

        builder.HasIndex(o => o.DeletedAt);

        builder.HasIndex(o => o.CreatedBy);

        // Add audit fields
        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false);

        builder.Property(o => o.DeletedAt)
            .IsRequired(false);

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.UpdatedBy)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.DeletedBy)
            .HasMaxLength(50)
            .IsRequired(false);

        // Add soft delete filter
        builder.HasQueryFilter(o => o.DeletedAt == null);
    }
} 
