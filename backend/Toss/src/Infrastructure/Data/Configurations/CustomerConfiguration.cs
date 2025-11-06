using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.CRM;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Customer entity.
/// Configures customer master data with CRM relationships and purchase history.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Required customer information
        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        // Monetary tracking
        builder.Property(c => c.TotalPurchaseAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        // Optional phone number as owned entity (value object pattern)
        builder.OwnsOne(c => c.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20);
        });

        // Relationships
        builder.HasOne(c => c.Shop)
            .WithMany()
            .HasForeignKey(c => c.ShopId)
            .OnDelete(DeleteBehavior.Cascade); // Delete customers if shop is deleted

        builder.HasOne(c => c.Address)
            .WithMany()
            .HasForeignKey(c => c.AddressId)
            .OnDelete(DeleteBehavior.SetNull); // Keep customer if address is deleted

        // Ignore computed property (alias for PurchaseHistory)
        builder.Ignore(c => c.Purchases);
        
        // Purchase history for analytics
        builder.HasMany(c => c.PurchaseHistory)
            .WithOne(cp => cp.Customer)
            .HasForeignKey(cp => cp.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // CRM interaction tracking
        builder.HasMany(c => c.Interactions)
            .WithOne(ci => ci.Customer)
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for lookups and filtering
        builder.HasIndex(c => new { c.ShopId, c.Email }); // Composite for shop customer lookup
        builder.HasIndex(c => c.LastPurchaseDate); // For RFM analysis
    }
}

