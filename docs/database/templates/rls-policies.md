# Row Level Security (RLS) Templates

## Overview
Row Level Security policies ensure multi-tenant data isolation at the database level. These templates provide standard RLS configurations for TOSS ERP III.

## Standard RLS Function

```sql
-- Create or replace the tenant access predicate function
CREATE OR REPLACE FUNCTION fn_tenant_access_predicate(tenant_id UUID)
RETURNS BOOLEAN AS $$
BEGIN
    -- Allow system operations (no tenant context)
    IF SESSION_USER = 'app_migration' OR SESSION_USER = 'postgres' THEN
        RETURN TRUE;
    END IF;
    
    -- Get current tenant from session variable
    DECLARE
        current_tenant_id UUID;
    BEGIN
        current_tenant_id := CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID);
        
        -- If no tenant context is set, deny access
        IF current_tenant_id IS NULL THEN
            RETURN FALSE;
        END IF;
        
        -- Allow access if tenant matches
        RETURN tenant_id = current_tenant_id;
    EXCEPTION
        WHEN OTHERS THEN
            -- If setting doesn't exist or conversion fails, deny access
            RETURN FALSE;
    END;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;
```

## Table RLS Policy Template

### Enable RLS on Table
```sql
-- Replace [TableName] with actual table name
ALTER TABLE "[TableName]" ENABLE ROW LEVEL SECURITY;
```

### Standard Tenant Policy
```sql
-- Policy for application service role
CREATE POLICY "[TableName]_tenant_policy" 
ON "[TableName]"
FOR ALL 
TO app_service
USING (fn_tenant_access_predicate("TenantId"));

-- Policy for read-only reporting role
CREATE POLICY "[TableName]_readonly_policy" 
ON "[TableName]"
FOR SELECT 
TO app_readonly
USING (fn_tenant_access_predicate("TenantId"));

-- Policy for system operations (migrations, maintenance)
CREATE POLICY "[TableName]_system_policy" 
ON "[TableName]"
FOR ALL 
TO app_migration
USING (true);
```

### Specialized Policies

#### Global Reference Data Policy
For tables that contain global reference data (TenantId = '00000000-0000-0000-0000-000000000000'):

```sql
CREATE POLICY "[TableName]_global_reference_policy" 
ON "[TableName]"
FOR SELECT 
TO app_service
USING (
    "TenantId" = '00000000-0000-0000-0000-000000000000'::UUID 
    OR fn_tenant_access_predicate("TenantId")
);
```

#### Audit Table Policy
For audit/log tables that may need cross-tenant access for compliance:

```sql
CREATE POLICY "[AuditTableName]_audit_policy" 
ON "[AuditTableName]"
FOR SELECT 
TO app_auditor
USING (
    -- Allow auditors to see specific tenant data based on their scope
    "TenantId" = ANY(
        SELECT unnest(
            string_to_array(
                CURRENT_SETTING('app.auditor_tenant_scope', true), 
                ','
            )::UUID[]
        )
    )
);
```

#### Time-based Access Policy
For tables with time-sensitive data:

```sql
CREATE POLICY "[TableName]_temporal_policy" 
ON "[TableName]"
FOR ALL 
TO app_service
USING (
    fn_tenant_access_predicate("TenantId") 
    AND (
        "ValidFrom" <= NOW() 
        AND ("ValidTo" IS NULL OR "ValidTo" >= NOW())
    )
);
```

## Complete RLS Setup Example

```sql
-- Example for Assets table
-- 1. Enable RLS
ALTER TABLE "Assets" ENABLE ROW LEVEL SECURITY;

-- 2. Create main tenant policy
CREATE POLICY "Assets_tenant_policy" 
ON "Assets"
FOR ALL 
TO app_service
USING (fn_tenant_access_predicate("TenantId"));

-- 3. Create read-only policy
CREATE POLICY "Assets_readonly_policy" 
ON "Assets"
FOR SELECT 
TO app_readonly
USING (fn_tenant_access_predicate("TenantId"));

-- 4. Create system policy
CREATE POLICY "Assets_system_policy" 
ON "Assets"
FOR ALL 
TO app_migration
USING (true);

-- 5. Test the policies
-- Set tenant context
SELECT set_config('app.current_tenant_id', '12345678-1234-1234-1234-123456789012', false);

-- Query should only return assets for the current tenant
SELECT * FROM "Assets";
```

## RLS Testing Queries

### Test Tenant Isolation
```sql
-- Set tenant context for tenant A
SELECT set_config('app.current_tenant_id', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', false);
INSERT INTO "Assets" ("Id", "TenantId", "Name", "Code", "CreatedAt", "CreatedBy", "LastModifiedAt") 
VALUES (gen_random_uuid(), 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Tenant A Asset', 'TA001', NOW(), 'TestUser', NOW());

-- Set tenant context for tenant B  
SELECT set_config('app.current_tenant_id', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', false);
INSERT INTO "Assets" ("Id", "TenantId", "Name", "Code", "CreatedAt", "CreatedBy", "LastModifiedAt") 
VALUES (gen_random_uuid(), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Tenant B Asset', 'TB001', NOW(), 'TestUser', NOW());

-- Switch back to tenant A - should only see tenant A assets
SELECT set_config('app.current_tenant_id', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', false);
SELECT "Name", "Code" FROM "Assets"; -- Should only return "Tenant A Asset"

-- Switch to tenant B - should only see tenant B assets
SELECT set_config('app.current_tenant_id', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', false);
SELECT "Name", "Code" FROM "Assets"; -- Should only return "Tenant B Asset"
```

### Test Cross-Tenant Access Prevention
```sql
-- Set tenant A context
SELECT set_config('app.current_tenant_id', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', false);

-- Try to insert data for tenant B - should fail with RLS policy
INSERT INTO "Assets" ("Id", "TenantId", "Name", "Code", "CreatedAt", "CreatedBy", "LastModifiedAt") 
VALUES (gen_random_uuid(), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Malicious Asset', 'MAL001', NOW(), 'TestUser', NOW());
-- Expected: Policy violation error
```

## C# DbContext Integration

```csharp
public class TenantDbContext : DbContext
{
    private readonly string _tenantId;
    
    public TenantDbContext(DbContextOptions<TenantDbContext> options, string tenantId) 
        : base(options)
    {
        _tenantId = tenantId;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(_tenantId))
        {
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.CommandTimeout(30);
                // Set tenant context when connection opens
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set tenant context before any database operations
        if (!string.IsNullOrEmpty(_tenantId))
        {
            await Database.ExecuteSqlRawAsync($"SELECT set_config('app.current_tenant_id', '{_tenantId}', false);", cancellationToken);
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    public override int SaveChanges()
    {
        // Set tenant context before any database operations
        if (!string.IsNullOrEmpty(_tenantId))
        {
            Database.ExecuteSqlRaw($"SELECT set_config('app.current_tenant_id', '{_tenantId}', false);");
        }
        
        return base.SaveChanges();
    }
}
```

## Database Role Setup

```sql
-- Create application roles
CREATE ROLE app_service LOGIN PASSWORD 'secure_password_here';
CREATE ROLE app_readonly LOGIN PASSWORD 'readonly_password_here';
CREATE ROLE app_migration LOGIN PASSWORD 'migration_password_here';
CREATE ROLE app_auditor LOGIN PASSWORD 'auditor_password_here';

-- Grant basic database access
GRANT CONNECT ON DATABASE tosserp TO app_service, app_readonly, app_migration, app_auditor;
GRANT USAGE ON SCHEMA public TO app_service, app_readonly, app_migration, app_auditor;

-- Grant table permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_service;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO app_readonly, app_auditor;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO app_migration;

-- Grant sequence permissions
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO app_service, app_migration;

-- Set default privileges for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app_service;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO app_readonly, app_auditor;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL PRIVILEGES ON TABLES TO app_migration;
```

## Monitoring RLS Policies

```sql
-- View all RLS policies
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles,
    cmd,
    qual,
    with_check
FROM pg_policies 
ORDER BY schemaname, tablename, policyname;

-- Check if RLS is enabled on tables
SELECT 
    schemaname,
    tablename,
    rowsecurity 
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY tablename;

-- Monitor RLS policy violations (requires log_statement = 'all')
-- Check PostgreSQL logs for policy violation messages
```

This RLS framework ensures complete data isolation between tenants while maintaining system flexibility for migrations and auditing.
