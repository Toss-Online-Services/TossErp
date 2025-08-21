namespace TossErp.Assets.Infrastructure.Data.Configurations;

/// <summary>
/// Asset Category entity configuration
/// </summary>
public class AssetCategoryConfiguration : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories", "assets");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Color)
            .HasMaxLength(7); // For hex color codes

        builder.Property(e => e.Icon)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Hierarchy
        builder.Property(e => e.ParentCategoryId);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Self-referencing relationship
        builder.HasOne(e => e.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(e => e.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasDatabaseName("IX_AssetCategories_TenantId_Code_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.Name })
            .HasDatabaseName("IX_AssetCategories_TenantId_Name")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.ParentCategoryId })
            .HasDatabaseName("IX_AssetCategories_TenantId_ParentCategoryId")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Asset Location entity configuration
/// </summary>
public class AssetLocationConfiguration : IEntityTypeConfiguration<AssetLocation>
{
    public void Configure(EntityTypeBuilder<AssetLocation> builder)
    {
        builder.ToTable("AssetLocations", "assets");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        // Address Information
        builder.Property(e => e.Building)
            .HasMaxLength(100);

        builder.Property(e => e.Floor)
            .HasMaxLength(50);

        builder.Property(e => e.Room)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        // GPS Coordinates
        builder.Property(e => e.Latitude)
            .HasPrecision(10, 8);

        builder.Property(e => e.Longitude)
            .HasPrecision(11, 8);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Hierarchy
        builder.Property(e => e.ParentLocationId);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Self-referencing relationship
        builder.HasOne(e => e.ParentLocation)
            .WithMany(l => l.SubLocations)
            .HasForeignKey(e => e.ParentLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.Code })
            .IsUnique()
            .HasDatabaseName("IX_AssetLocations_TenantId_Code_Unique")
            .HasFillFactor(95);

        builder.HasIndex(e => new { e.TenantId, e.Name })
            .HasDatabaseName("IX_AssetLocations_TenantId_Name")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.ParentLocationId })
            .HasDatabaseName("IX_AssetLocations_TenantId_ParentLocationId")
            .HasFillFactor(90);
    }
}
