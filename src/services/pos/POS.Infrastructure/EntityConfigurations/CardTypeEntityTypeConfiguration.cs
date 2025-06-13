using eShop.POS.Domain.AggregatesModel.BuyerAggregate;

namespace eShop.POS.Infrastructure.EntityConfigurations;

class CardTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypesConfiguration)
    {
        cardTypesConfiguration.ToTable("cardtypes");

        cardTypesConfiguration.Property(ct => ct.Id)
            .HasMaxLength(36)
            .IsRequired();

        cardTypesConfiguration.Property(ct => ct.Name)
            .HasMaxLength(200)
            .IsRequired();

        cardTypesConfiguration.HasIndex(ct => ct.Name)
            .IsUnique(true);
    }
}
