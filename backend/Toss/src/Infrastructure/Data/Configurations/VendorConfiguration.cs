using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Vendors;

namespace Toss.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for the unified Vendor entity (merged from Supplier and Vendor)
/// </summary>
public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CompanyRegNumber)
            .HasMaxLength(50);

        builder.Property(x => x.VATNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(200);

        builder.Property(x => x.Website)
            .HasMaxLength(500);

        builder.Property(x => x.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(x => x.AdminComment)
            .HasMaxLength(4000);

        // Optional ContactPhone using OwnsOne for nullable support
        builder.OwnsOne(x => x.ContactPhone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20);
        });

        builder.HasOne(x => x.Address)
            .WithMany()
            .HasForeignKey(x => x.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.VendorNotes)
            .WithOne(x => x.Vendor)
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.VendorProducts)
            .WithOne(x => x.Vendor)
            .HasForeignKey(x => x.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.Deleted);
        builder.HasIndex(x => x.DisplayOrder);
    }
}

