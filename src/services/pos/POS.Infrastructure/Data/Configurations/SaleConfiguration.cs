using POS.Domain.AggregatesModel.SaleAggregate;

namespace eShop.POS.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StoreId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.StaffId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.StaffName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CustomerId)
            .HasMaxLength(50);

        builder.Property(s => s.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.RefundReason)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsMany(s => s.Items, itemBuilder =>
        {
            itemBuilder.ToTable("SaleItems");
            itemBuilder.WithOwner().HasForeignKey("SaleId");
            itemBuilder.Property(i => i.ProductId).IsRequired().HasMaxLength(50);
            itemBuilder.Property(i => i.ProductName).IsRequired().HasMaxLength(100);
            itemBuilder.Property(i => i.UnitPrice).HasPrecision(18, 2);
            itemBuilder.Property(i => i.Category).IsRequired().HasMaxLength(50);
            itemBuilder.Property(i => i.Barcode).HasMaxLength(50);
            itemBuilder.Property(i => i.Variant).HasMaxLength(50);
        });

        builder.OwnsMany(s => s.Payments, paymentBuilder =>
        {
            paymentBuilder.ToTable("Payments");
            paymentBuilder.WithOwner().HasForeignKey("SaleId");
            paymentBuilder.Property(p => p.Method).IsRequired().HasConversion<string>();
            paymentBuilder.Property(p => p.Amount).HasPrecision(18, 2);
            paymentBuilder.Property(p => p.Reference).HasMaxLength(100);
            paymentBuilder.Property(p => p.CardLast4).HasMaxLength(4);
            paymentBuilder.Property(p => p.CardType).HasMaxLength(50);
        });

        builder.OwnsMany(s => s.Discounts, discountBuilder =>
        {
            discountBuilder.ToTable("SaleDiscounts");
            discountBuilder.WithOwner().HasForeignKey("SaleId");
            discountBuilder.Property(d => d.Amount).HasPrecision(18, 2);
            discountBuilder.Property(d => d.Reason).IsRequired().HasMaxLength(200);
            discountBuilder.Property(d => d.Type).IsRequired().HasConversion<string>();
            discountBuilder.Property(d => d.StaffId).HasMaxLength(50);
        });

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);
    }
} 
