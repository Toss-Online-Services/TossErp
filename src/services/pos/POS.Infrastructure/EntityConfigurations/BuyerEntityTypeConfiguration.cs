using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eShop.POS.Domain.AggregatesModel.BuyerAggregate;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace eShop.POS.Infrastructure.EntityConfigurations;

class BuyerEntityTypeConfiguration
    : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> buyerConfiguration)
    {
        buyerConfiguration.ToTable("buyers");

        buyerConfiguration.Ignore(b => b.DomainEvents);

        buyerConfiguration.Property(b => b.Id)
            .UseHiLo("buyerseq", "POS");

        buyerConfiguration.Property(b => b.IdentityGuid)
            .HasMaxLength(200);

        buyerConfiguration.HasIndex("IdentityGuid")
            .IsUnique(true);

        buyerConfiguration.HasMany(b => b.PaymentMethods)
            .WithOne();
    }
}
