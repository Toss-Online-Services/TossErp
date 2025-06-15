using POS.Domain.AggregatesModel.BuyerAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("buyers", "POS");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasConversion<string>();

        builder.Property(b => b.StoreId).HasConversion<string>().IsRequired();

        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.Email).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Phone).HasMaxLength(20).IsRequired();
        builder.Property(b => b.TaxId).HasMaxLength(50);
        builder.Property(b => b.Notes).HasMaxLength(500);
        builder.Property(b => b.Status).HasMaxLength(50).IsRequired();

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt);

        builder.HasIndex(b => b.StoreId);
        builder.HasIndex(b => b.Email).IsUnique();
        builder.HasIndex(b => b.Phone).IsUnique();
        builder.HasIndex(b => b.TaxId).IsUnique();
        builder.HasIndex(b => b.Status);

        builder.OwnsOne(b => b.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200).IsRequired();
            a.Property(addr => addr.City).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.State).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.Country).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.ZipCode).HasMaxLength(20).IsRequired();
        });

        builder.HasMany(b => b.PaymentMethods)
            .WithOne()
            .HasForeignKey("BuyerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Store)
            .WithMany()
            .HasForeignKey(b => b.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
