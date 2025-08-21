namespace TossErp.Assets.Infrastructure.Data;

/// <summary>
/// Multi-tenant Asset Management DbContext with EF Core 9 optimizations
/// </summary>
public sealed class AssetsDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;
    private readonly ILogger<AssetsDbContext> _logger;

    public AssetsDbContext(
        DbContextOptions<AssetsDbContext> options,
        ICurrentTenantService currentTenantService,
        ILogger<AssetsDbContext> logger) 
        : base(options)
    {
        _currentTenantService = currentTenantService;
        _logger = logger;
    }

    // Asset Management Entities
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<AssetLocation> AssetLocations => Set<AssetLocation>();
    public DbSet<AssetTransfer> AssetTransfers => Set<AssetTransfer>();
    public DbSet<MaintenanceSchedule> MaintenanceSchedules => Set<MaintenanceSchedule>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<AssetDocument> AssetDocuments => Set<AssetDocument>();
    public DbSet<AssetAuditLog> AssetAuditLogs => Set<AssetAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetsDbContext).Assembly);

        // Apply migration helper configurations
        MigrationHelper.SeedAssetCategories(modelBuilder);
        MigrationHelper.SeedAssetLocations(modelBuilder);
        MigrationHelper.ConfigureDatabaseOptimizations(modelBuilder);
        MigrationHelper.ConfigureTriggers(modelBuilder);

        // Configure multi-tenant global query filters
        ConfigureGlobalQueryFilters(modelBuilder);

        // Configure database optimizations for EF Core 9
        ConfigureDatabaseOptimizations(modelBuilder);
    }

    private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        var tenantId = _currentTenantService.TenantId;

        // Apply tenant filter to all tenant-aware entities
        modelBuilder.Entity<Asset>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AssetCategory>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AssetLocation>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AssetTransfer>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<MaintenanceSchedule>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<MaintenanceRecord>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AssetDocument>()
            .HasQueryFilter(e => e.TenantId == tenantId);
        
        modelBuilder.Entity<AssetAuditLog>()
            .HasQueryFilter(e => e.TenantId == tenantId);
    }

    private static void ConfigureDatabaseOptimizations(ModelBuilder modelBuilder)
    {
        // Configure indexes for performance
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Status })
                .HasDatabaseName("IX_Assets_TenantId_Status")
                .HasFillFactor(90);
            
            entity.HasIndex(e => new { e.TenantId, e.AssetTag })
                .IsUnique()
                .HasDatabaseName("IX_Assets_TenantId_AssetTag_Unique")
                .HasFillFactor(95);
            
            entity.HasIndex(e => new { e.TenantId, e.CategoryId })
                .HasDatabaseName("IX_Assets_TenantId_CategoryId")
                .HasFillFactor(90);
            
            entity.HasIndex(e => new { e.TenantId, e.LocationId })
                .HasDatabaseName("IX_Assets_TenantId_LocationId")
                .HasFillFactor(90);
        });

        modelBuilder.Entity<AssetTransfer>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.AssetId, e.TransferDate })
                .HasDatabaseName("IX_AssetTransfers_TenantId_AssetId_TransferDate")
                .HasFillFactor(85);
        });

        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.AssetId, e.ScheduledDate })
                .HasDatabaseName("IX_MaintenanceRecords_TenantId_AssetId_ScheduledDate")
                .HasFillFactor(85);
        });

        modelBuilder.Entity<AssetAuditLog>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.AssetId, e.Timestamp })
                .HasDatabaseName("IX_AssetAuditLogs_TenantId_AssetId_Timestamp")
                .HasFillFactor(80);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set tenant ID for new entities
        SetTenantIdForNewEntities();
        
        // Set audit fields
        SetAuditFields();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Set tenant ID for new entities
        SetTenantIdForNewEntities();
        
        // Set audit fields
        SetAuditFields();
        
        return base.SaveChanges();
    }

    private void SetTenantIdForNewEntities()
    {
        var tenantId = _currentTenantService.TenantId;
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Entity is ITenantEntity tenantEntity)
            {
                tenantEntity.TenantId = tenantId;
            }
        }
    }

    private void SetAuditFields()
    {
        var userId = _currentTenantService.UserId;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedBy = userId;
                        break;
                    
                    case EntityState.Modified:
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedBy = userId;
                        break;
                }
            }
        }
    }
}

/// <summary>
/// Interface for tenant-aware entities
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}

/// <summary>
/// Interface for auditable entities
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
