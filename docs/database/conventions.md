# Database Conventions & Standards

## Table of Contents
- [Overview](#overview)
- [Naming Conventions](#naming-conventions)
- [Schema Design](#schema-design)
- [Multi-Tenancy with RLS](#multi-tenancy-with-rls)
- [Migration Standards](#migration-standards)
- [Indexing Strategy](#indexing-strategy)
- [Security & Permissions](#security--permissions)
- [Performance Guidelines](#performance-guidelines)

## Overview

This document establishes consistent database conventions for the TOSS ERP III platform. All services must follow these standards to ensure maintainability, performance, and security across the entire system.

## Naming Conventions

### Tables
- Use PascalCase for table names: `Assets`, `StockMovements`, `AssetCategories`
- Use descriptive, business-domain names
- Avoid abbreviations unless widely understood
- For junction tables, combine entity names: `ItemSuppliers`, `AssetLocations`

### Columns
- Use PascalCase for column names: `TenantId`, `CreatedAt`, `IsActive`
- Use meaningful, descriptive names
- Boolean columns should start with `Is`, `Has`, or `Can`: `IsActive`, `HasExpiry`, `CanTransfer`
- Foreign key columns should end with `Id`: `TenantId`, `CategoryId`, `LocationId`

### Indexes
- Use descriptive names with IX_ prefix: `IX_Assets_TenantId_Status`
- Include table name and key columns: `IX_TableName_Column1_Column2`
- For unique indexes, use UX_ prefix: `UX_Assets_TenantId_AssetTag`

### Constraints
- Check constraints: `CK_TableName_Description`: `CK_Assets_PurchasePrice`
- Foreign keys: `FK_ChildTable_ParentTable_ColumnName`: `FK_Assets_Categories_CategoryId`
- Primary keys: `PK_TableName`: `PK_Assets`
- Unique constraints: `UC_TableName_Description`: `UC_Users_Email`

## Schema Design

### Standard Columns
Every entity table must include these standard columns:

```sql
-- Primary Key
Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),

-- Multi-tenancy
TenantId UNIQUEIDENTIFIER NOT NULL,

-- Audit Trail
CreatedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
CreatedBy NVARCHAR(100) NOT NULL,
LastModifiedAt DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
LastModifiedBy NVARCHAR(100) NULL,

-- Soft Delete (if applicable)
IsDeleted BIT NOT NULL DEFAULT 0,
DeletedAt DATETIMEOFFSET NULL,
DeletedBy NVARCHAR(100) NULL,

-- Status/Activation (if applicable)
IsActive BIT NOT NULL DEFAULT 1
```

### Entity Framework Configuration
All entities should implement standard interfaces:

```csharp
public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTimeOffset LastModifiedAt { get; set; }
    string? LastModifiedBy { get; set; }
}

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
```

## Multi-Tenancy with RLS

### Row Level Security Policy Template

```sql
-- Enable RLS on table
ALTER TABLE [dbo].[TableName] ENABLE ROW LEVEL SECURITY;

-- Create security policy
CREATE SECURITY POLICY [TableName]SecurityPolicy
ON [dbo].[TableName]
ADD FILTER PREDICATE [dbo].[fn_TenantAccessPredicate]([TenantId]) = 1,
ADD BLOCK PREDICATE [dbo].[fn_TenantAccessPredicate]([TenantId]) = 1 ON AFTER INSERT,
ADD BLOCK PREDICATE [dbo].[fn_TenantAccessPredicate]([TenantId]) = 1 ON AFTER UPDATE
WITH (STATE = ON);
```

### Tenant Access Function

```sql
CREATE OR REPLACE FUNCTION fn_TenantAccessPredicate(tenant_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    -- Allow system operations (no tenant context)
    IF SESSION_USER = 'system' THEN
        RETURN TRUE;
    END IF;
    
    -- Check if user has access to this tenant
    RETURN tenant_id = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID);
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;
```

### C# Context Configuration

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!string.IsNullOrEmpty(_tenantId))
    {
        optionsBuilder.UseNpgsql(_connectionString, options =>
        {
            options.CommandTimeout(30);
            options.SetPostgresVersion(new Version(13, 0));
        });
    }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply tenant filter globally
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
        {
            var method = SetGlobalQueryMethod.MakeGenericMethod(entityType.ClrType);
            method.Invoke(this, new object[] { modelBuilder, entityType.ClrType });
        }
    }
}

private void SetGlobalQuery<T>(ModelBuilder builder, Type entityType) where T : class, ITenantEntity
{
    builder.Entity<T>().HasQueryFilter(e => e.TenantId == _tenantId);
}
```

## Migration Standards

### Migration File Structure

```csharp
public partial class DescriptiveMigrationName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Create tables
        CreateTables(migrationBuilder);
        
        // 2. Create indexes
        CreateIndexes(migrationBuilder);
        
        // 3. Add constraints
        AddConstraints(migrationBuilder);
        
        // 4. Seed reference data
        SeedReferenceData(migrationBuilder);
        
        // 5. Enable RLS policies
        EnableRowLevelSecurity(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse operations in opposite order
        DisableRowLevelSecurity(migrationBuilder);
        RemoveConstraints(migrationBuilder);
        DropIndexes(migrationBuilder);
        DropTables(migrationBuilder);
    }
    
    private void CreateTables(MigrationBuilder migrationBuilder) { }
    private void CreateIndexes(MigrationBuilder migrationBuilder) { }
    private void AddConstraints(MigrationBuilder migrationBuilder) { }
    private void SeedReferenceData(MigrationBuilder migrationBuilder) { }
    private void EnableRowLevelSecurity(MigrationBuilder migrationBuilder) { }
}
```

### Migration Helper Template

```csharp
public static class MigrationHelper
{
    public static void SeedReferenceData<T>(ModelBuilder modelBuilder, T[] data) 
        where T : class
    {
        modelBuilder.Entity<T>().HasData(data);
    }

    public static void ConfigureStandardColumns<T>(EntityTypeBuilder<T> builder) 
        where T : class, IAuditableEntity, ITenantEntity
    {
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastModifiedAt).IsRequired();
        builder.Property(e => e.LastModifiedBy).HasMaxLength(100);
        
        builder.HasIndex(e => e.TenantId).HasDatabaseName($"IX_{typeof(T).Name}_TenantId");
    }

    public static void ConfigureSoftDelete<T>(EntityTypeBuilder<T> builder) 
        where T : class, ISoftDeletable
    {
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.HasIndex(e => e.IsDeleted).HasDatabaseName($"IX_{typeof(T).Name}_IsDeleted");
    }

    public static void CreateStandardIndexes<T>(ModelBuilder modelBuilder, string tableName)
        where T : class, ITenantEntity
    {
        modelBuilder.Entity<T>()
            .HasIndex(e => e.TenantId)
            .HasDatabaseName($"IX_{tableName}_TenantId")
            .HasFillFactor(90);
    }
}
```

## Indexing Strategy

### Standard Indexes Required

1. **Tenant Isolation Index** (Every multi-tenant table):
   ```sql
   CREATE INDEX IX_TableName_TenantId ON TableName (TenantId) WITH FILLFACTOR = 90;
   ```

2. **Composite Tenant Indexes** (For filtered queries):
   ```sql
   CREATE INDEX IX_TableName_TenantId_Status ON TableName (TenantId, Status) WITH FILLFACTOR = 90;
   ```

3. **Audit Trail Indexes**:
   ```sql
   CREATE INDEX IX_TableName_CreatedAt ON TableName (CreatedAt) WITH FILLFACTOR = 80;
   CREATE INDEX IX_TableName_LastModifiedAt ON TableName (LastModifiedAt) WITH FILLFACTOR = 80;
   ```

### Performance Guidelines

- Use **FILLFACTOR = 90** for frequently updated tables
- Use **FILLFACTOR = 80** for heavily inserted tables (audit logs)
- Use **FILLFACTOR = 95** for mostly read-only reference data
- Always include TenantId as the first column in composite indexes

## Security & Permissions

### Database Roles

```sql
-- Application role for service connections
CREATE ROLE app_service;
GRANT CONNECT ON DATABASE tosserp TO app_service;
GRANT USAGE ON SCHEMA public TO app_service;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_service;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO app_service;

-- Read-only role for reporting
CREATE ROLE app_readonly;
GRANT CONNECT ON DATABASE tosserp TO app_readonly;
GRANT USAGE ON SCHEMA public TO app_readonly;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO app_readonly;

-- Migration role for schema changes
CREATE ROLE app_migration;
GRANT ALL PRIVILEGES ON DATABASE tosserp TO app_migration;
```

### Connection String Template

```json
{
  "ConnectionStrings": {
    "TossErpDb": "Host=localhost;Database=tosserp;Username=app_service;Password={password};Include Error Detail=true;Trust Server Certificate=true"
  }
}
```

## Performance Guidelines

### Query Patterns

1. **Always filter by TenantId first**:
   ```sql
   WHERE TenantId = @TenantId AND Status = 'Active'
   ```

2. **Use covering indexes for complex queries**:
   ```sql
   CREATE INDEX IX_Orders_TenantId_Status_Date_COVERING 
   ON Orders (TenantId, Status, OrderDate) 
   INCLUDE (TotalAmount, CustomerId);
   ```

3. **Batch operations for large datasets**:
   ```csharp
   var batchSize = 1000;
   var batches = items.Chunk(batchSize);
   foreach (var batch in batches)
   {
       context.Items.AddRange(batch);
       await context.SaveChangesAsync();
   }
   ```

### Monitoring & Alerts

- Monitor slow queries > 5 seconds
- Alert on blocked processes > 30 seconds
- Track database size growth > 80% capacity
- Monitor connection pool exhaustion
- Alert on failed RLS policy violations

## Environment-Specific Considerations

### Development
- Use local PostgreSQL instance
- Enable detailed logging
- Relax some constraints for testing

### Staging
- Mirror production configuration
- Use masked production data
- Enable query performance monitoring

### Production
- Enable all security policies
- Implement backup strategies
- Configure connection pooling
- Set up read replicas for reporting

## Compliance & Auditing

### Audit Requirements
- All data modifications must be tracked
- User actions must be logged with timestamps
- System changes must be attributed
- Regulatory compliance (GDPR, SOX) considerations

### Data Retention
- Audit logs: 7 years
- Transaction data: 5 years
- User activity: 2 years
- System logs: 1 year

This document serves as the foundation for all database development in the TOSS ERP III platform. All developers must review and follow these conventions to ensure consistency and maintainability.
