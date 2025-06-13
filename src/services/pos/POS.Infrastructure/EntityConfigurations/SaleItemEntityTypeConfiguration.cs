namespace TossErp.POS.Infrastructure.EntityConfigurations;

class SaleItemEntityTypeConfiguration
    : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> saleItemConfiguration)
    {
        saleItemConfiguration.ToTable("sale_items", "POS");

        saleItemConfiguration.Property(si => si.Id)
            .HasMaxLength(36)
            .IsRequired();

        saleItemConfiguration.Property(si => si.ProductId)
            .HasMaxLength(36)
            .IsRequired();

        saleItemConfiguration.Property(si => si.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        saleItemConfiguration.Property(si => si.Quantity)
            .IsRequired();

        saleItemConfiguration.Property(si => si.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        saleItemConfiguration.Property(si => si.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleItemConfiguration.Property(si => si.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleItemConfiguration.Property(si => si.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleItemConfiguration.HasIndex(si => si.ProductId);
    }
} 
