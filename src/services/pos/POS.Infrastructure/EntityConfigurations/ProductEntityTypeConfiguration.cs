namespace TossErp.POS.Infrastructure.EntityConfigurations;

class ProductEntityTypeConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> productConfiguration)
    {
        productConfiguration.ToTable("products", "POS");

        productConfiguration.Ignore(p => p.DomainEvents);

        productConfiguration.Property(p => p.Id)
            .HasMaxLength(36)
            .IsRequired();

        productConfiguration.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();

        productConfiguration.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        productConfiguration.Property(p => p.Description)
            .HasMaxLength(500);

        productConfiguration.Property(p => p.Category)
            .HasMaxLength(100);

        productConfiguration.Property(p => p.Brand)
            .HasMaxLength(100);

        productConfiguration.Property(p => p.StoreId)
            .HasMaxLength(36)
            .IsRequired();

        productConfiguration.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        productConfiguration.Property(p => p.StockQuantity)
            .IsRequired();

        productConfiguration.HasIndex(p => new { p.StoreId, p.Code })
            .IsUnique();

        productConfiguration.HasIndex(p => p.StoreId);
    }
} 
