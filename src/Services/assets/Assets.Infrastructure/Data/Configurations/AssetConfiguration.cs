namespace TossErp.Assets.Infrastructure.Data.Configurations;

/// <summary>
/// Asset entity configuration with EF Core 9 optimizations
/// </summary>
public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", "assets");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Asset Tag (unique per tenant)
        builder.Property(e => e.AssetTag)
            .IsRequired()
            .HasMaxLength(50);

        // Basic Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.SerialNumber)
            .HasMaxLength(100);

        builder.Property(e => e.Model)
            .HasMaxLength(100);

        builder.Property(e => e.Manufacturer)
            .HasMaxLength(100);

        // Status and Condition
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Condition)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Financial Information (using EF Core 9 complex types)
        builder.OwnsOne(e => e.FinancialInfo, fb =>
        {
            fb.Property(f => f.PurchasePrice)
                .HasPrecision(18, 2)
                .HasColumnName("PurchasePrice");

            fb.Property(f => f.CurrentValue)
                .HasPrecision(18, 2)
                .HasColumnName("CurrentValue");

            fb.Property(f => f.DepreciationRate)
                .HasPrecision(5, 4)
                .HasColumnName("DepreciationRate");

            fb.Property(f => f.SalvageValue)
                .HasPrecision(18, 2)
                .HasColumnName("SalvageValue");

            fb.Property(f => f.DepreciationMethod)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnName("DepreciationMethod");
        });

        // Dates
        builder.Property(e => e.PurchaseDate)
            .IsRequired();

        builder.Property(e => e.InServiceDate);

        builder.Property(e => e.WarrantyExpiration);

        builder.Property(e => e.LastMaintenanceDate);

        builder.Property(e => e.NextMaintenanceDue);

        // Maintenance Information
        builder.Property(e => e.MaintenanceNotes)
            .HasMaxLength(2000);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.Category)
            .WithMany(c => c.Assets)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Location)
            .WithMany(l => l.Assets)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Transfers)
            .WithOne(t => t.Asset)
            .HasForeignKey(t => t.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.MaintenanceRecords)
            .WithOne(m => m.Asset)
            .HasForeignKey(m => m.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.Asset)
            .HasForeignKey(d => d.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.AuditLogs)
            .WithOne(a => a.Asset)
            .HasForeignKey(a => a.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes with fill factors (EF Core 9 feature)
        builder.HasIndex(e => new { e.TenantId, e.AssetTag })
            .IsUnique()
            .HasDatabaseName("IX_Assets_TenantId_AssetTag_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_Assets_TenantId_Status")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.CategoryId })
            .HasDatabaseName("IX_Assets_TenantId_CategoryId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.LocationId })
            .HasDatabaseName("IX_Assets_TenantId_LocationId")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.SerialNumber })
            .HasDatabaseName("IX_Assets_TenantId_SerialNumber")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.PurchaseDate })
            .HasDatabaseName("IX_Assets_TenantId_PurchaseDate")
            .HasFillFactor(85);
    }
}
