using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.CustomerAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .UseHiLo("customerseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastModifiedAt);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.Notes)
            .HasMaxLength(500);

        builder.Property(c => c.LastPurchaseDate);

        builder.Property(c => c.TotalPurchases)
            .HasPrecision(18, 2);

        builder.Property(c => c.PurchaseCount);

        builder.Property(c => c.CustomerType)
            .IsRequired();

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.PhoneNumber)
            .IsUnique();

        // Configure value objects
        builder.OwnsOne(c => c.Preferences, preferences =>
        {
            preferences.Property(p => p.ReceiveEmailNotifications).IsRequired();
            preferences.Property(p => p.ReceiveSMSNotifications).IsRequired();
            preferences.Property(p => p.ReceivePostalMail).IsRequired();
            preferences.Property(p => p.PreferredLanguage).IsRequired().HasMaxLength(10);
            preferences.Property(p => p.PreferredCurrency).IsRequired().HasMaxLength(3);
            preferences.Property(p => p.PreferredPaymentMethod).HasMaxLength(50);
            preferences.Property(p => p.DietaryRestrictions).HasMaxLength(200);
        });

        // Configure relationships
        builder.HasMany(c => c.PriceLists)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Documents)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.CustomerNotes)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
