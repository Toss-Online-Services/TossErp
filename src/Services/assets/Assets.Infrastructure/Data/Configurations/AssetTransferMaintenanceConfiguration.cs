namespace TossErp.Assets.Infrastructure.Data.Configurations;

/// <summary>
/// Asset Transfer entity configuration
/// </summary>
public class AssetTransferConfiguration : IEntityTypeConfiguration<AssetTransfer>
{
    public void Configure(EntityTypeBuilder<AssetTransfer> builder)
    {
        builder.ToTable("AssetTransfers", "assets");

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

        builder.Property(e => e.FromLocationId);
        builder.Property(e => e.ToLocationId)
            .IsRequired();

        // Transfer Details
        builder.Property(e => e.TransferDate)
            .IsRequired();

        builder.Property(e => e.TransferReason)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.TransferredBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(e => e.ApprovedAt);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.FromLocation)
            .WithMany()
            .HasForeignKey(e => e.FromLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ToLocation)
            .WithMany()
            .HasForeignKey(e => e.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AssetId, e.TransferDate })
            .HasDatabaseName("IX_AssetTransfers_TenantId_AssetId_TransferDate")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_AssetTransfers_TenantId_Status")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Maintenance Schedule entity configuration
/// </summary>
public class MaintenanceScheduleConfiguration : IEntityTypeConfiguration<MaintenanceSchedule>
{
    public void Configure(EntityTypeBuilder<MaintenanceSchedule> builder)
    {
        builder.ToTable("MaintenanceSchedules", "assets");

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

        // Schedule Details
        builder.Property(e => e.MaintenanceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.FrequencyType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.FrequencyValue)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate);

        builder.Property(e => e.LastPerformed);
        builder.Property(e => e.NextDue)
            .IsRequired();

        builder.Property(e => e.EstimatedDuration)
            .IsRequired();

        builder.Property(e => e.EstimatedCost)
            .HasPrecision(18, 2);

        builder.Property(e => e.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.AssignedTo)
            .HasMaxLength(256);

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
        builder.HasIndex(e => new { e.TenantId, e.AssetId, e.NextDue })
            .HasDatabaseName("IX_MaintenanceSchedules_TenantId_AssetId_NextDue")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.NextDue, e.IsActive })
            .HasDatabaseName("IX_MaintenanceSchedules_TenantId_NextDue_IsActive")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.AssignedTo })
            .HasDatabaseName("IX_MaintenanceSchedules_TenantId_AssignedTo")
            .HasFillFactor(90);
    }
}

/// <summary>
/// Maintenance Record entity configuration
/// </summary>
public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.ToTable("MaintenanceRecords", "assets");

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

        builder.Property(e => e.ScheduleId);

        // Maintenance Details
        builder.Property(e => e.MaintenanceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.ScheduledDate)
            .IsRequired();

        builder.Property(e => e.StartedAt);
        builder.Property(e => e.CompletedAt);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(e => e.PerformedBy)
            .HasMaxLength(256);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2);

        builder.Property(e => e.Notes)
            .HasMaxLength(2000);

        builder.Property(e => e.NextMaintenanceDue);

        // Audit Properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.Schedule)
            .WithMany(s => s.MaintenanceRecords)
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.AssetId, e.ScheduledDate })
            .HasDatabaseName("IX_MaintenanceRecords_TenantId_AssetId_ScheduledDate")
            .HasFillFactor(85);

        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_MaintenanceRecords_TenantId_Status")
            .HasFillFactor(90);

        builder.HasIndex(e => new { e.TenantId, e.PerformedBy })
            .HasDatabaseName("IX_MaintenanceRecords_TenantId_PerformedBy")
            .HasFillFactor(90);
    }
}
