using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.AggregatesModel.StoreAggregate;

namespace POS.Infrastructure.EntityConfigurations;

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores", POSContext.DEFAULT_SCHEMA);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .UseHiLo("storseq", POSContext.DEFAULT_SCHEMA);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        // Configure Address value object
        builder.OwnsOne(s => s.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200);
            a.Property(addr => addr.City).HasMaxLength(100);
            a.Property(addr => addr.State).HasMaxLength(100);
            a.Property(addr => addr.Country).HasMaxLength(100);
            a.Property(addr => addr.ZipCode).HasMaxLength(20);
        });

        builder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Website)
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.TaxId)
            .HasMaxLength(50);

        builder.Property(s => s.LicenseNumber)
            .HasMaxLength(50);

        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);

        builder.Property(s => s.BannerUrl)
            .HasMaxLength(500);

        builder.Property(s => s.SocialMediaLinks)
            .HasMaxLength(1000);

        builder.Property(s => s.TimeZone)
            .IsRequired()
            .HasMaxLength(50);

        // Configure Settings value object
        builder.OwnsOne(s => s.Settings, settings =>
        {
            settings.Property(s => s.TaxRate)
                .IsRequired()
                .HasPrecision(5, 2);
        });

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Configure collections
        builder.HasMany(s => s.StoreHours)
            .WithOne()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Devices)
            .WithOne()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Printers)
            .WithOne()
            .HasForeignKey("StoreId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(s => s.Code)
            .IsUnique();

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.HasIndex(s => s.Phone);

        builder.HasIndex(s => s.IsActive);

        builder.HasIndex(s => s.Name);

        builder.HasIndex(s => s.UpdatedAt);

        builder.HasIndex(s => s.TimeZone);

        builder.HasIndex(s => s.TaxId);
    }
} 
