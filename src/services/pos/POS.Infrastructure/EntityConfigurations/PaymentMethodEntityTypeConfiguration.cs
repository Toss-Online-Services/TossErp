using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.BuyerAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.BuyerId).IsRequired();
        builder.Property(x => x.CardNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CardHolderName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ExpirationDate).IsRequired().HasMaxLength(10);
        builder.Property(x => x.SecurityCode).HasMaxLength(10);
        builder.Property(x => x.Alias).HasMaxLength(100);
        builder.Property(x => x.CardTypeId).IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.BuyerId);
        builder.HasIndex(x => x.CardNumber);
        builder.HasIndex(x => x.CardTypeId);
        builder.HasIndex(x => x.IsDefault);
        builder.HasIndex(x => x.IsActive);

        builder.HasOne(x => x.Buyer)
            .WithMany(x => x.PaymentMethods)
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CardType)
            .WithMany()
            .HasForeignKey(x => x.CardTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
