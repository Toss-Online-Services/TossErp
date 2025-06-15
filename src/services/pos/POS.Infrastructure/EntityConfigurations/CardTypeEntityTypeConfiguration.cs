using POS.Domain.AggregatesModel.BuyerAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> builder)
    {
        builder.ToTable("card_types", "POS");

        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Id)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(ct => ct.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ct => ct.Description)
            .HasMaxLength(200);

        builder.Property(ct => ct.CreatedAt)
            .IsRequired();

        builder.Property(ct => ct.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(ct => ct.Name).IsUnique();
    }
}
