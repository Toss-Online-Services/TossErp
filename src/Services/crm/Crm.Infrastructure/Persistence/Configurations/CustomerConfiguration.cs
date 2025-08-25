using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever(); // We set the GUID in the domain

        // Basic properties
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.DateOfBirth)
            .IsRequired();

        // Enum properties
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Segment)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Date properties
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastPurchaseDate);

        // Numeric properties
        builder.Property(c => c.TotalSpent)
            .IsRequired();

        builder.Property(c => c.PurchaseCount)
            .IsRequired();

        builder.Property(c => c.LoyaltyPoints)
            .IsRequired();

        // Relationships
        builder.HasMany(c => c.Interactions)
            .WithOne()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.LoyaltyTransactions)
            .WithOne()
            .HasForeignKey(lt => lt.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes (simplified for InMemory)
        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Segment);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.LastPurchaseDate);

        // Computed properties (read-only)
        builder.Ignore(c => c.FullName);
        builder.Ignore(c => c.IsLapsed);
        builder.Ignore(c => c.IsHighValue);
    }
}
