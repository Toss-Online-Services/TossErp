namespace TossErp.POS.Infrastructure.EntityConfigurations;

class PaymentMethodEntityTypeConfiguration
    : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> paymentConfiguration)
    {
        paymentConfiguration.ToTable("paymentmethods");

        paymentConfiguration.Ignore(b => b.DomainEvents);

        paymentConfiguration.Property(b => b.Id)
            .HasMaxLength(36)
            .IsRequired();

        paymentConfiguration.Property<string>("BuyerId")
            .HasMaxLength(36)
            .IsRequired();

        paymentConfiguration
            .Property("_cardHolderName")
            .HasColumnName("CardHolderName")
            .HasMaxLength(200);

        paymentConfiguration
            .Property("_alias")
            .HasColumnName("Alias")
            .HasMaxLength(200);

        paymentConfiguration
            .Property("_cardNumber")
            .HasColumnName("CardNumber")
            .HasMaxLength(25)
            .IsRequired();

        paymentConfiguration
            .Property("_expiration")
            .HasColumnName("Expiration")
            .HasMaxLength(25);

        paymentConfiguration
            .Property("_cardTypeId")
            .HasColumnName("CardTypeId")
            .HasMaxLength(36);

        paymentConfiguration.HasOne(p => p.CardType)
            .WithMany()
            .HasForeignKey("_cardTypeId");
    }
}
