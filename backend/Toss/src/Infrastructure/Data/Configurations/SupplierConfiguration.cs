using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Suppliers;

namespace Toss.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.CompanyRegNumber)
            .HasMaxLength(50);

        builder.Property(s => s.VATNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Email)
            .HasMaxLength(256);

        builder.Property(s => s.Website)
            .HasMaxLength(500);

        builder.Property(s => s.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        builder.ComplexProperty(s => s.ContactPhone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20);
        });

        builder.HasOne(s => s.Address)
            .WithMany()
            .HasForeignKey(s => s.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.Email);
    }
}

