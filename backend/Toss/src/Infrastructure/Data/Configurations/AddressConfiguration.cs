using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities;

namespace Toss.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(a => a.Street)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.City)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasMaxLength(2)
            .IsRequired();

        // Optional Location coordinates using OwnsOne for nullable support
        builder.OwnsOne(a => a.Coordinates, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude);
            locationBuilder.Property(l => l.Longitude);
            locationBuilder.Property(l => l.Area).HasMaxLength(200);
            locationBuilder.Property(l => l.Zone).HasMaxLength(200);
        });
    }
}

