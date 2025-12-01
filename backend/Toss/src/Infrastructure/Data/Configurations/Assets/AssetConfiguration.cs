using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Assets;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data.Configurations.Assets;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Code)
            .HasMaxLength(100);

        builder.Property(a => a.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(a => a.PurchaseCost)
            .HasPrecision(18, 2);

        builder.Property(a => a.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Condition)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Category)
            .HasMaxLength(100);

        builder.Property(a => a.Brand)
            .HasMaxLength(100);

        builder.Property(a => a.Model)
            .HasMaxLength(100);

        builder.Property(a => a.SerialNumber)
            .HasMaxLength(100);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Shop)
            .WithMany()
            .HasForeignKey(a => a.ShopId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(a => a.MaintenanceLogs)
            .WithOne(m => m.Asset)
            .HasForeignKey(m => m.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.BusinessId);
        builder.HasIndex(a => a.ShopId);
        builder.HasIndex(a => a.Code);
        builder.HasIndex(a => a.Condition);
        builder.HasIndex(a => new { a.BusinessId, a.IsActive });
    }
}

