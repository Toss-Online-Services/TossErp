using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities;

namespace Toss.Infrastructure.Data.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(s => s.Language)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.Timezone)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Email)
            .HasMaxLength(256);

        // Location as owned type to support nullable (optional)
        builder.OwnsOne(s => s.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.Latitude).IsRequired();
            locationBuilder.Property(l => l.Longitude).IsRequired();
            locationBuilder.Property(l => l.Area).HasMaxLength(200);
            locationBuilder.Property(l => l.Zone).HasMaxLength(200);
        });

        // Optional ContactPhone using OwnsOne for nullable support
        builder.OwnsOne(s => s.ContactPhone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Number).HasMaxLength(20);
        });

        builder.HasIndex(s => s.OwnerId);
        builder.HasIndex(s => s.AreaGroup);
    }
}

