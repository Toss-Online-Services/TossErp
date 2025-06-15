using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.StoreAggregate;

namespace TossErp.POS.Infrastructure.EntityConfigurations;

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores", "POS");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion<string>();

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(20).IsRequired();
        builder.Property(s => s.TaxNumber).HasMaxLength(50);
        builder.Property(s => s.Notes).HasMaxLength(500);

        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.IsSynced).IsRequired();

        builder.HasIndex(s => s.Code).IsUnique();
        builder.HasIndex(s => s.Email).IsUnique();
        builder.HasIndex(s => s.Phone).IsUnique();
        builder.HasIndex(s => s.TaxNumber).IsUnique();
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.IsSynced);

        builder.OwnsOne(s => s.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200).IsRequired();
            a.Property(addr => addr.City).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.State).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.Country).HasMaxLength(100).IsRequired();
            a.Property(addr => addr.ZipCode).HasMaxLength(20).IsRequired();
        });
    }
} 
