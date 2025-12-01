using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toss.Domain.Entities.Assets;

namespace Toss.Infrastructure.Data.Configurations.Assets;

public class AssetMaintenanceLogConfiguration : IEntityTypeConfiguration<AssetMaintenanceLog>
{
    public void Configure(EntityTypeBuilder<AssetMaintenanceLog> builder)
    {
        builder.ToTable("AssetMaintenanceLogs");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MaintenanceDate)
            .IsRequired();

        builder.Property(m => m.MaintenanceType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(m => m.Cost)
            .HasPrecision(18, 2);

        builder.Property(m => m.ServiceProvider)
            .HasMaxLength(200);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(m => m.Business)
            .WithMany()
            .HasForeignKey(m => m.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Asset)
            .WithMany(a => a.MaintenanceLogs)
            .HasForeignKey(m => m.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => m.BusinessId);
        builder.HasIndex(m => m.AssetId);
        builder.HasIndex(m => m.MaintenanceDate);
        builder.HasIndex(m => new { m.AssetId, m.MaintenanceDate });
    }
}

