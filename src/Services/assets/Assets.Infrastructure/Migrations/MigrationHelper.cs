namespace TossErp.Assets.Infrastructure.Migrations;

/// <summary>
/// Migration helper for seeding initial data
/// </summary>
public static class MigrationHelper
{
    /// <summary>
    /// Seeds initial asset categories for all tenants
    /// </summary>
    public static void SeedAssetCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new AssetCategory
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                TenantId = Guid.Empty, // Global category template
                Name = "Computer Equipment",
                Code = "COMP",
                Description = "Computers, laptops, servers, and related IT equipment",
                Color = "#007ACC",
                Icon = "computer",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new AssetCategory
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                TenantId = Guid.Empty,
                Name = "Office Furniture",
                Code = "FURN",
                Description = "Desks, chairs, cabinets, and office furniture",
                Color = "#8B4513",
                Icon = "chair",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new AssetCategory
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                TenantId = Guid.Empty,
                Name = "Vehicles",
                Code = "VHCL",
                Description = "Company vehicles and transportation equipment",
                Color = "#FF6B35",
                Icon = "car",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new AssetCategory
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                TenantId = Guid.Empty,
                Name = "Machinery & Equipment",
                Code = "MACH",
                Description = "Manufacturing equipment, tools, and machinery",
                Color = "#FF8C00",
                Icon = "cog",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new AssetCategory
            {
                Id = new Guid("55555555-5555-5555-5555-555555555555"),
                TenantId = Guid.Empty,
                Name = "Software & Licenses",
                Code = "SOFT",
                Description = "Software licenses, applications, and digital assets",
                Color = "#6A5ACD",
                Icon = "code",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        };

        modelBuilder.Entity<AssetCategory>().HasData(categories);
    }

    /// <summary>
    /// Seeds initial asset locations
    /// </summary>
    public static void SeedAssetLocations(ModelBuilder modelBuilder)
    {
        var locations = new[]
        {
            new AssetLocation
            {
                Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                TenantId = Guid.Empty,
                Name = "Main Office",
                Code = "MAIN",
                Description = "Primary office location",
                Address = "123 Business Street",
                City = "Business City",
                State = "BC",
                Country = "Country",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new AssetLocation
            {
                Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                TenantId = Guid.Empty,
                Name = "Warehouse",
                Code = "WARE",
                Description = "Storage and warehouse facility",
                Address = "456 Storage Avenue",
                City = "Storage City",
                State = "SC",
                Country = "Country",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        };

        modelBuilder.Entity<AssetLocation>().HasData(locations);
    }

    /// <summary>
    /// Configure index creation with proper naming and performance optimizations
    /// </summary>
    public static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Asset indexes with fill factors for performance
        modelBuilder.Entity<Asset>()
            .HasIndex(a => new { a.TenantId, a.AssetTag })
            .HasDatabaseName("IX_Assets_TenantId_AssetTag")
            .IsUnique()
            .HasFillFactor(90);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => new { a.TenantId, a.Status })
            .HasDatabaseName("IX_Assets_TenantId_Status")
            .HasFillFactor(90);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => new { a.TenantId, a.CategoryId })
            .HasDatabaseName("IX_Assets_TenantId_CategoryId")
            .HasFillFactor(90);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => new { a.TenantId, a.LocationId })
            .HasDatabaseName("IX_Assets_TenantId_LocationId")
            .HasFillFactor(90);

        // Maintenance schedule indexes for background processing
        modelBuilder.Entity<MaintenanceSchedule>()
            .HasIndex(m => new { m.TenantId, m.NextMaintenanceDate, m.IsActive })
            .HasDatabaseName("IX_MaintenanceSchedules_TenantId_NextDate_Active")
            .HasFillFactor(90);

        // Asset transfer indexes for tracking
        modelBuilder.Entity<AssetTransfer>()
            .HasIndex(t => new { t.TenantId, t.TransferDate })
            .HasDatabaseName("IX_AssetTransfers_TenantId_TransferDate")
            .HasFillFactor(90);

        // Document indexes for file management
        modelBuilder.Entity<AssetDocument>()
            .HasIndex(d => new { d.TenantId, d.AssetId, d.DocumentType })
            .HasDatabaseName("IX_AssetDocuments_TenantId_AssetId_Type")
            .HasFillFactor(90);

        // Audit log indexes for reporting
        modelBuilder.Entity<AssetAuditLog>()
            .HasIndex(a => new { a.TenantId, a.AssetId, a.Timestamp })
            .HasDatabaseName("IX_AssetAuditLogs_TenantId_AssetId_Timestamp")
            .HasFillFactor(80); // Lower fill factor for frequently inserted data
    }

    /// <summary>
    /// Configure database-specific optimizations
    /// </summary>
    public static void ConfigureDatabaseOptimizations(ModelBuilder modelBuilder)
    {
        // Configure check constraints for data integrity
        modelBuilder.Entity<Asset>()
            .ToTable(t => t.HasCheckConstraint("CK_Assets_PurchasePrice", "[FinancialInfo_PurchasePrice] >= 0"));

        modelBuilder.Entity<Asset>()
            .ToTable(t => t.HasCheckConstraint("CK_Assets_AccumulatedDepreciation", "[FinancialInfo_AccumulatedDepreciation] >= 0"));

        modelBuilder.Entity<Asset>()
            .ToTable(t => t.HasCheckConstraint("CK_Assets_UsefulLifeYears", "[FinancialInfo_UsefulLifeYears] > 0"));

        // Configure temporal tables for audit trails (SQL Server 2016+)
        if (IsSqlServer(modelBuilder))
        {
            modelBuilder.Entity<Asset>()
                .ToTable(t => t.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("AssetsHistory");
                    ttb.HasPeriodStart("PeriodStart").HasColumnName("PeriodStart");
                    ttb.HasPeriodEnd("PeriodEnd").HasColumnName("PeriodEnd");
                }));
        }
    }

    /// <summary>
    /// Configure triggers for audit logging
    /// </summary>
    public static void ConfigureTriggers(ModelBuilder modelBuilder)
    {
        // Note: Triggers would be configured in migrations or via raw SQL
        // This is a placeholder for trigger configuration logic
    }

    /// <summary>
    /// Determines if the database provider is SQL Server
    /// </summary>
    private static bool IsSqlServer(ModelBuilder modelBuilder)
    {
        // This would be determined at runtime based on the database provider
        return true; // Placeholder
    }
}

/// <summary>
/// Extension methods for migration operations
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Applies seed data for a specific tenant
    /// </summary>
    public static async Task SeedTenantData(this AssetsDbContext context, Guid tenantId)
    {
        // Copy global categories to tenant-specific categories
        var globalCategories = await context.AssetCategories
            .Where(c => c.TenantId == Guid.Empty)
            .ToListAsync();

        foreach (var globalCategory in globalCategories)
        {
            var tenantCategory = new AssetCategory
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = globalCategory.Name,
                Code = globalCategory.Code,
                Description = globalCategory.Description,
                Color = globalCategory.Color,
                Icon = globalCategory.Icon,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.AssetCategories.Add(tenantCategory);
        }

        // Copy global locations to tenant-specific locations
        var globalLocations = await context.AssetLocations
            .Where(l => l.TenantId == Guid.Empty)
            .ToListAsync();

        foreach (var globalLocation in globalLocations)
        {
            var tenantLocation = new AssetLocation
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = globalLocation.Name,
                Code = globalLocation.Code,
                Description = globalLocation.Description,
                Address = globalLocation.Address,
                City = globalLocation.City,
                State = globalLocation.State,
                PostalCode = globalLocation.PostalCode,
                Country = globalLocation.Country,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.AssetLocations.Add(tenantLocation);
        }

        await context.SaveChangesAsync();
    }
}
