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

        // Configure CustomerName value object
        builder.OwnsOne(c => c._name, name =>
        {
            name.Property(n => n.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            name.Property(n => n.LastName)
                .IsRequired()
                .HasMaxLength(100);
        });

        // Configure ContactInfo value object
        builder.OwnsOne(c => c._contactInfo, contact =>
        {
            contact.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);
            contact.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
        });

        // Configure CreditLimit value object
        builder.OwnsOne(c => c._creditLimit, credit =>
        {
            credit.Property(c => c.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
        });

        // Configure PaymentTerms value object
        builder.OwnsOne(c => c._paymentTerms, terms =>
        {
            terms.Property(t => t.Days)
                .IsRequired();
            terms.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(50);
        });

        // Configure CustomerBalance value object
        builder.OwnsOne(c => c._balance, balance =>
        {
            balance.Property(b => b.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            balance.Property(b => b.Currency)
                .IsRequired()
                .HasMaxLength(3);
        });

        // Configure Address value object
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.State).HasMaxLength(100);
            address.Property(a => a.Country).HasMaxLength(100);
            address.Property(a => a.PostalCode).HasMaxLength(20);
        });

        // Configure LoyaltyProgram value object
        builder.OwnsOne(c => c.LoyaltyProgram, loyalty =>
        {
            loyalty.Property(l => l.Id).IsRequired();
            loyalty.Property(l => l.Name).IsRequired().HasMaxLength(100);
            loyalty.Property(l => l.MembershipNumber).IsRequired().HasMaxLength(50);
            loyalty.Property(l => l.MembershipTier).IsRequired().HasMaxLength(50);
            loyalty.Property(l => l.Points).IsRequired();
            loyalty.Property(l => l.EnrollmentDate).IsRequired();
        });

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

        // Configure value objects
        builder.OwnsOne(c => c.Preferences, preferences =>
        {
            preferences.Property(p => p.ReceiveEmailNotifications).IsRequired();
            preferences.Property(p => p.ReceiveSMSNotifications).IsRequired();
            preferences.Property(p => p.ReceivePostalMail).IsRequired();
            preferences.Property(p => p.PreferredLanguage).IsRequired().HasMaxLength(10);
            preferences.Property(p => p.PreferredCurrency).IsRequired().HasMaxLength(3);
            preferences.Property(p => p.PreferredPaymentMethod).HasMaxLength(50);
            preferences.Property(p => p.PreferredShippingMethod).HasMaxLength(50);
            preferences.Property(p => p.OptInMarketing).IsRequired();
            preferences.Property(p => p.OptInThirdParty).IsRequired();
            preferences.Property(p => p.DietaryRestrictions).HasMaxLength(200);
            preferences.Property(p => p.SpecialInstructions).HasMaxLength(500);
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

        // Configure indexes
        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.PhoneNumber)
            .IsUnique();

        builder.HasIndex(c => c.CustomerType);

        builder.HasIndex(c => c.IsActive);

        builder.HasIndex(c => c.LastPurchaseDate);

        builder.HasIndex(c => c.TotalPurchases);
    }
} 
