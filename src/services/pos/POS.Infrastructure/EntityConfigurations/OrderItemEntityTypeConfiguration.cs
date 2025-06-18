using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.OrderAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .UseHiLo("orderitemseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oi => oi.ProductSku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // Configure Money value objects
        builder.OwnsOne(oi => oi.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.OwnsOne(oi => oi.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            money.Property(m => m.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Property(oi => oi.Notes)
            .HasMaxLength(500);

        // Configure Order relationship
        builder.HasOne<Order>()
            .WithMany(o => o.OrderItems)
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
