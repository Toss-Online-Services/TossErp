using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.CRM;

namespace Toss.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.TotalPurchaseAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        // Optional PhoneNumber using OwnsOne for nullable support
        builder.OwnsOne(c => c.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20);
        });

        builder.HasOne(c => c.Shop)
            .WithMany()
            .HasForeignKey(c => c.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Address)
            .WithMany()
            .HasForeignKey(c => c.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.PurchaseHistory)
            .WithOne(cp => cp.Customer)
            .HasForeignKey(cp => cp.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Interactions)
            .WithOne(ci => ci.Customer)
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.ShopId, c.Email });
        builder.HasIndex(c => c.LastPurchaseDate);
    }
}

