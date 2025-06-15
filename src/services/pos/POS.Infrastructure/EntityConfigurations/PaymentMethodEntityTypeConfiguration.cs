using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.BuyerAggregate;


namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods", "POS");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion<string>();

        builder.Property(p => p.StoreId).HasConversion<string>().IsRequired();

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Type).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.IsSynced).IsRequired();

        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.IsSynced);

        builder.HasOne(p => p.Store)
            .WithMany()
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
