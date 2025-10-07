using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TossErp.Domain.Entities.Sales;

namespace TossErp.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.Email)
            .HasMaxLength(100);
        
        builder.HasIndex(c => c.Email)
            .HasFilter("[Email] IS NOT NULL");
        
        builder.Property(c => c.Phone)
            .HasMaxLength(20);
        
        builder.HasIndex(c => c.Phone)
            .HasFilter("[Phone] IS NOT NULL");
        
        builder.Property(c => c.AlternatePhone)
            .HasMaxLength(20);
        
        builder.Property(c => c.AddressLine1)
            .HasMaxLength(200);
        
        builder.Property(c => c.AddressLine2)
            .HasMaxLength(200);
        
        builder.Property(c => c.City)
            .HasMaxLength(100);
        
        builder.Property(c => c.Province)
            .HasMaxLength(100);
        
        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);
        
        builder.Property(c => c.Country)
            .HasMaxLength(100);
        
        builder.Property(c => c.CompanyName)
            .HasMaxLength(200);
        
        builder.Property(c => c.TaxNumber)
            .HasMaxLength(50);
        
        builder.Property(c => c.RegistrationNumber)
            .HasMaxLength(50);
        
        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.CurrentBalance)
            .HasPrecision(18, 2);
        
        builder.Property(c => c.LoyaltyTier)
            .HasMaxLength(50);
        
        builder.Property(c => c.PreferredPaymentMethod)
            .HasMaxLength(50);
        
        builder.Property(c => c.PreferredLanguage)
            .HasMaxLength(50);
        
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.IsVip);
        
        // Relationships
        builder.HasMany(c => c.Sales)
            .WithOne()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

