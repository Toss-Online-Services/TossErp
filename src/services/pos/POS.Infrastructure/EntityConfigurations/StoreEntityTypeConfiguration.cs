using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.StoreAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores", "POS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.TaxId).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Website).HasMaxLength(200);
        builder.Property(x => x.Logo).HasMaxLength(500);
        builder.Property(x => x.Currency).HasMaxLength(10);
        builder.Property(x => x.TimeZone).HasMaxLength(50);
        builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsSynced).IsRequired();
        builder.Property(x => x.SyncedAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200).IsRequired();
            a.Property(addr => addr.City).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.State).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.Country).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.ZipCode).HasMaxLength(20).IsRequired();
        });

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.TaxId).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Phone).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.IsSynced);
    }
} 
