using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.StaffAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class SaleDiscountEntityTypeConfiguration : IEntityTypeConfiguration<SaleDiscount>
{
    public void Configure(EntityTypeBuilder<SaleDiscount> builder)
    {
        builder.ToTable("sale_discounts", "POS");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion<string>().IsRequired();

        builder.Property(s => s.SaleId).HasConversion<string>().IsRequired();
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Type).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Reason).HasMaxLength(200);
        builder.Property(s => s.StaffId).HasConversion<string>();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);

        // Indexes
        builder.HasIndex(s => s.SaleId);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.StaffId);

        // Relationships
        builder.HasOne(s => s.Sale)
            .WithMany(s => s.Discounts)
            .HasForeignKey(s => s.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Staff)
            .WithMany()
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 
