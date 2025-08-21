namespace TossErp.Assets.Infrastructure.Data.Configurations;

/// <summary>
/// Asset Document entity configuration
/// </summary>
public class AssetDocumentConfiguration : IEntityTypeConfiguration<AssetDocument>
{
    public void Configure(EntityTypeBuilder<AssetDocument> builder)
    {
        builder.ToTable("AssetDocuments", "assets");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.AssetId)
            .IsRequired();

        // Document Properties
        builder.Property(e => e.DocumentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.Property(e => e.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.UploadedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.UploadedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AssetId, e.DocumentType })
            .HasDatabaseName("IX_AssetDocuments_TenantId_AssetId_DocumentType")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.UploadedBy, e.UploadedAt })
            .HasDatabaseName("IX_AssetDocuments_TenantId_UploadedBy_UploadedAt")
            .HasFillFactor(85);
    }
}

/// <summary>
/// Asset Audit Log entity configuration
/// </summary>
public class AssetAuditLogConfiguration : IEntityTypeConfiguration<AssetAuditLog>
{
    public void Configure(EntityTypeBuilder<AssetAuditLog> builder)
    {
        builder.ToTable("AssetAuditLogs", "assets");

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Tenant
        builder.Property(e => e.TenantId)
            .IsRequired();

        // Foreign Keys
        builder.Property(e => e.AssetId)
            .IsRequired();

        // Audit Properties
        builder.Property(e => e.Action)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.TableName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EntityId)
            .IsRequired();

        builder.Property(e => e.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.AffectedColumns)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.PrimaryKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.UserName)
            .HasMaxLength(256);

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.Property(e => e.IpAddress)
            .HasMaxLength(45); // For IPv6

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        // Indexes optimized for audit log queries
        builder.HasIndex(e => new { e.TenantId, e.AssetId, e.Timestamp })
            .HasDatabaseName("IX_AssetAuditLogs_TenantId_AssetId_Timestamp")
            .HasFillFactor(80);

        builder.HasIndex(e => new { e.TenantId, e.Action, e.Timestamp })
            .HasDatabaseName("IX_AssetAuditLogs_TenantId_Action_Timestamp")
            .HasFillFactor(80);

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.Timestamp })
            .HasDatabaseName("IX_AssetAuditLogs_TenantId_UserId_Timestamp")
            .HasFillFactor(80);

        builder.HasIndex(e => new { e.TenantId, e.TableName, e.EntityId, e.Timestamp })
            .HasDatabaseName("IX_AssetAuditLogs_TenantId_TableName_EntityId_Timestamp")
            .HasFillFactor(75);
    }
}
