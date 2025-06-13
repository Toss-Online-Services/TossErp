namespace TossErp.POS.Infrastructure.EntityConfigurations;

class SaleEntityTypeConfiguration
    : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> saleConfiguration)
    {
        saleConfiguration.ToTable("sales", "POS");

        saleConfiguration.Ignore(s => s.DomainEvents);

        saleConfiguration.Property(s => s.Id)
            .HasMaxLength(36)
            .IsRequired();

        saleConfiguration.Property(s => s.StoreId)
            .HasMaxLength(36)
            .IsRequired();

        saleConfiguration.Property(s => s.StaffId)
            .HasMaxLength(36)
            .IsRequired();

        saleConfiguration.Property(s => s.BuyerId)
            .HasMaxLength(36);

        saleConfiguration.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        saleConfiguration.Property(s => s.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleConfiguration.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleConfiguration.Property(s => s.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleConfiguration.Property(s => s.FinalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        saleConfiguration.Property(s => s.PaymentMethodId)
            .HasMaxLength(36)
            .IsRequired();

        saleConfiguration.Property(s => s.CardTypeId)
            .HasMaxLength(36);

        saleConfiguration.Property(s => s.CardNumber)
            .HasMaxLength(50);

        saleConfiguration.Property(s => s.Notes)
            .HasMaxLength(500);

        saleConfiguration.Property(s => s.CreatedAt)
            .IsRequired();

        saleConfiguration.Property(s => s.UpdatedAt)
            .IsRequired();

        saleConfiguration.HasIndex(s => s.StoreId);
        saleConfiguration.HasIndex(s => s.StaffId);
        saleConfiguration.HasIndex(s => s.BuyerId);
        saleConfiguration.HasIndex(s => s.Status);
        saleConfiguration.HasIndex(s => s.CreatedAt);

        saleConfiguration.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
