using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.BuyerAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.ToTable("CardTypes", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Icon).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.IsActive);
    }
}
