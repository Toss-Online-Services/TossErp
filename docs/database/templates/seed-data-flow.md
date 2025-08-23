# Seed Data Flow Templates

## Overview
This document provides standardized patterns for seeding reference data across different environments in TOSS ERP III.

## Environment-Specific Seed Strategies

### Development Environment
- Full reference data with realistic test values
- Sample business data for development testing
- Multiple test tenants with varied configurations

### Staging Environment
- Production-like reference data
- Masked/anonymized production data subsets
- Limited test tenants mirroring production

### Production Environment
- Essential reference data only
- No test data
- Real tenant configurations

## Seed Data Categories

### 1. Global Reference Data
Data that applies across all tenants (TenantId = '00000000-0000-0000-0000-000000000000')

### 2. Tenant Template Data
Default configurations that are copied when new tenants are created

### 3. Environment-Specific Data
Data that varies based on deployment environment

### 4. Migration Seed Data
Data required for schema migrations and system functionality

## Seed Data Implementation Patterns

### Pattern 1: Migration-Based Seeding

```csharp
public static class MigrationSeedHelper
{
    public static void SeedGlobalReferenceData<T>(
        MigrationBuilder migrationBuilder, 
        string tableName,
        T[] data) where T : class
    {
        var properties = typeof(T).GetProperties();
        var columns = properties.Select(p => p.Name).ToArray();
        
        var values = new object[data.Length, properties.Length];
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < properties.Length; j++)
            {
                values[i, j] = properties[j].GetValue(data[i]);
            }
        }
        
        migrationBuilder.InsertData(
            table: tableName,
            columns: columns,
            values: values
        );
    }
    
    public static void SeedCurrencies(MigrationBuilder migrationBuilder)
    {
        var currencies = new[]
        {
            new { 
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                TenantId = Guid.Empty,
                Code = "USD", 
                Name = "US Dollar", 
                Symbol = "$",
                IsActive = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System",
                LastModifiedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new { 
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                TenantId = Guid.Empty,
                Code = "EUR", 
                Name = "Euro", 
                Symbol = "€",
                IsActive = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System",
                LastModifiedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        };
        
        SeedGlobalReferenceData(migrationBuilder, "Currencies", currencies);
    }
}
```

### Pattern 2: DbContext-Based Seeding

```csharp
public static class DbContextSeedExtensions
{
    public static async Task SeedAsync(this DbContext context, ILogger logger, string environment)
    {
        try
        {
            await SeedGlobalReferenceDataAsync(context, logger);
            
            if (environment == "Development")
            {
                await SeedDevelopmentDataAsync(context, logger);
            }
            else if (environment == "Staging")
            {
                await SeedStagingDataAsync(context, logger);
            }
            
            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
    
    private static async Task SeedGlobalReferenceDataAsync(DbContext context, ILogger logger)
    {
        // Seed currencies if not exists
        if (!await context.Set<Currency>().AnyAsync())
        {
            await context.Set<Currency>().AddRangeAsync(GetDefaultCurrencies());
            logger.LogInformation("Seeded default currencies");
        }
        
        // Seed countries if not exists
        if (!await context.Set<Country>().AnyAsync())
        {
            await context.Set<Country>().AddRangeAsync(GetDefaultCountries());
            logger.LogInformation("Seeded default countries");
        }
    }
    
    private static async Task SeedDevelopmentDataAsync(DbContext context, ILogger logger)
    {
        // Create development test tenants
        await SeedTestTenantsAsync(context, logger);
        
        // Create sample business data
        await SeedSampleBusinessDataAsync(context, logger);
    }
}
```

### Pattern 3: Tenant-Specific Seeding

```csharp
public static class TenantSeedService
{
    public static async Task SeedNewTenantAsync(
        DbContext context, 
        Guid tenantId, 
        TenantConfiguration config,
        ILogger logger)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            // Set tenant context
            await context.Database.ExecuteSqlRawAsync(
                $"SELECT set_config('app.current_tenant_id', '{tenantId}', false)");
            
            // Copy global reference data to tenant
            await CopyGlobalReferenceDataAsync(context, tenantId, logger);
            
            // Create tenant-specific default data
            await CreateTenantDefaultsAsync(context, tenantId, config, logger);
            
            // Create default admin user for tenant
            await CreateTenantAdminAsync(context, tenantId, config, logger);
            
            await transaction.CommitAsync();
            logger.LogInformation("Successfully seeded new tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to seed tenant {TenantId}", tenantId);
            throw;
        }
    }
    
    private static async Task CopyGlobalReferenceDataAsync(
        DbContext context, 
        Guid tenantId, 
        ILogger logger)
    {
        // Copy asset categories
        var globalCategories = await context.Set<AssetCategory>()
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
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "System",
                LastModifiedAt = DateTimeOffset.UtcNow
            };
            
            context.Set<AssetCategory>().Add(tenantCategory);
        }
        
        logger.LogInformation("Copied {Count} asset categories for tenant {TenantId}", 
            globalCategories.Count, tenantId);
    }
}
```

## Environment Configuration

### appsettings.Development.json
```json
{
  "SeedData": {
    "Enabled": true,
    "IncludeTestData": true,
    "TestTenantCount": 3,
    "SampleDataSize": "Medium",
    "Features": {
      "CreateSampleUsers": true,
      "CreateSampleTransactions": true,
      "CreateSampleAssets": true
    }
  }
}
```

### appsettings.Staging.json
```json
{
  "SeedData": {
    "Enabled": true,
    "IncludeTestData": false,
    "TestTenantCount": 1,
    "SampleDataSize": "Small",
    "Features": {
      "CreateSampleUsers": false,
      "CreateSampleTransactions": false,
      "CreateSampleAssets": false
    }
  }
}
```

### appsettings.Production.json
```json
{
  "SeedData": {
    "Enabled": false,
    "IncludeTestData": false,
    "TestTenantCount": 0,
    "SampleDataSize": "None",
    "Features": {
      "CreateSampleUsers": false,
      "CreateSampleTransactions": false,
      "CreateSampleAssets": false
    }
  }
}
```

## Standard Reference Data Sets

### Currencies
```csharp
public static class StandardCurrencies
{
    public static Currency[] GetDefaultCurrencies() => new[]
    {
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "USD", Name = "US Dollar", Symbol = "$", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "EUR", Name = "Euro", Symbol = "€", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "GBP", Name = "British Pound", Symbol = "£", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "JPY", Name = "Japanese Yen", Symbol = "¥", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "AUD", Name = "Australian Dollar", Symbol = "A$", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "CHF", Name = "Swiss Franc", Symbol = "Fr", IsActive = true },
        new Currency { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "CNY", Name = "Chinese Yuan", Symbol = "¥", IsActive = true }
    };
}
```

### Units of Measure
```csharp
public static class StandardUnitsOfMeasure
{
    public static UnitOfMeasure[] GetDefaultUnits() => new[]
    {
        // Length
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "m", Name = "Meter", Category = "Length", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "cm", Name = "Centimeter", Category = "Length", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "mm", Name = "Millimeter", Category = "Length", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "in", Name = "Inch", Category = "Length", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "ft", Name = "Foot", Category = "Length", IsActive = true },
        
        // Weight
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "kg", Name = "Kilogram", Category = "Weight", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "g", Name = "Gram", Category = "Weight", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "lb", Name = "Pound", Category = "Weight", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "oz", Name = "Ounce", Category = "Weight", IsActive = true },
        
        // Volume
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "l", Name = "Liter", Category = "Volume", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "ml", Name = "Milliliter", Category = "Volume", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "gal", Name = "Gallon", Category = "Volume", IsActive = true },
        
        // Quantity
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "pcs", Name = "Pieces", Category = "Quantity", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "doz", Name = "Dozen", Category = "Quantity", IsActive = true },
        new UnitOfMeasure { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "box", Name = "Box", Category = "Quantity", IsActive = true }
    };
}
```

### Tax Codes
```csharp
public static class StandardTaxCodes
{
    public static TaxCode[] GetDefaultTaxCodes() => new[]
    {
        new TaxCode { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "VAT_STD", Name = "Standard VAT", Rate = 20.0m, IsActive = true },
        new TaxCode { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "VAT_RED", Name = "Reduced VAT", Rate = 5.0m, IsActive = true },
        new TaxCode { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "VAT_ZERO", Name = "Zero VAT", Rate = 0.0m, IsActive = true },
        new TaxCode { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "EXEMPT", Name = "Tax Exempt", Rate = 0.0m, IsActive = true },
        new TaxCode { Id = Guid.NewGuid(), TenantId = Guid.Empty, Code = "SALES_TAX", Name = "Sales Tax", Rate = 8.5m, IsActive = true }
    };
}
```

## Seed Data Validation

```csharp
public static class SeedDataValidator
{
    public static async Task ValidateSeedDataAsync(DbContext context, ILogger logger)
    {
        var validationResults = new List<string>();
        
        // Validate required reference data exists
        await ValidateRequiredDataAsync<Currency>(context, "Currencies", validationResults);
        await ValidateRequiredDataAsync<Country>(context, "Countries", validationResults);
        await ValidateRequiredDataAsync<UnitOfMeasure>(context, "Units of Measure", validationResults);
        
        // Validate data integrity
        await ValidateDataIntegrityAsync(context, validationResults);
        
        if (validationResults.Any())
        {
            foreach (var result in validationResults)
            {
                logger.LogWarning("Seed data validation issue: {Issue}", result);
            }
        }
        else
        {
            logger.LogInformation("Seed data validation passed");
        }
    }
    
    private static async Task ValidateRequiredDataAsync<T>(
        DbContext context, 
        string entityName, 
        List<string> validationResults) where T : class
    {
        var count = await context.Set<T>().CountAsync();
        if (count == 0)
        {
            validationResults.Add($"No {entityName} found in database");
        }
    }
    
    private static async Task ValidateDataIntegrityAsync(DbContext context, List<string> validationResults)
    {
        // Check for orphaned records
        var orphanedCategories = await context.Set<AssetCategory>()
            .Where(c => c.TenantId != Guid.Empty)
            .Where(c => !context.Set<Tenant>().Any(t => t.Id == c.TenantId))
            .CountAsync();
            
        if (orphanedCategories > 0)
        {
            validationResults.Add($"{orphanedCategories} asset categories have invalid tenant references");
        }
    }
}
```

## Automated Seeding in Startup

```csharp
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider, 
        ILogger<DatabaseInitializer> logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        
        try
        {
            // Ensure database exists and is up to date
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed");
            
            // Seed data if enabled
            if (configuration.GetValue<bool>("SeedData:Enabled"))
            {
                await context.SeedAsync(logger, environment.EnvironmentName);
                logger.LogInformation("Database seeding completed");
            }
            
            // Validate seed data
            await SeedDataValidator.ValidateSeedDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}

// In Program.cs
var app = builder.Build();

// Initialize database on startup
await DatabaseInitializer.InitializeAsync(app.Services, app.Services.GetRequiredService<ILogger<DatabaseInitializer>>());
```

This seed data framework ensures consistent reference data across all environments while maintaining tenant isolation and environment-specific configurations.
