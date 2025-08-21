using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table configuration
        builder.ToTable("Customers");

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
            .IsRequired()
            .HasColumnType("date");

        // Enum properties
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Segment)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Audit properties
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.LastPurchaseDate)
            .HasColumnType("timestamp with time zone");

        // Financial properties
        builder.Property(c => c.TotalSpent)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(c => c.PurchaseCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.LoyaltyPoints)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasMany(c => c.Interactions)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.LoyaltyTransactions)
            .WithOne()
            .HasForeignKey("CustomerId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasDatabaseName("IX_Customer_Email");

        builder.HasIndex(c => c.Phone)
            .HasDatabaseName("IX_Customer_Phone");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Customer_Status");

        builder.HasIndex(c => c.Segment)
            .HasDatabaseName("IX_Customer_Segment");

        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Customer_CreatedAt");

        builder.HasIndex(c => c.LastPurchaseDate)
            .HasDatabaseName("IX_Customer_LastPurchaseDate");

        builder.HasIndex(c => c.TotalSpent)
            .HasDatabaseName("IX_Customer_TotalSpent");

        // Computed properties (read-only)
        builder.Ignore(c => c.FullName);
        builder.Ignore(c => c.IsLapsed);
        builder.Ignore(c => c.IsHighValue);
    }
}
