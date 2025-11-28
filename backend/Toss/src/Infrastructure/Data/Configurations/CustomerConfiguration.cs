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

        builder.Property(c => c.Tags)
            .HasMaxLength(512);

        // Monetary tracking
        builder.Property(c => c.TotalPurchaseAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        // Optional phone number as owned entity (value object pattern)
        builder.OwnsOne(c => c.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);
        });

        // Relationships
        builder.HasOne(c => c.Business)
            .WithMany()
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Store)
            .WithMany()
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.SetNull);

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
        builder.Property<string>("PhoneNumber");

        builder.HasIndex(c => new { c.BusinessId, c.Email });
        builder.HasIndex(new[] { "BusinessId", "PhoneNumber" }).IsUnique();
        builder.HasIndex(c => c.LastPurchaseDate); // For RFM analysis
    }
}

