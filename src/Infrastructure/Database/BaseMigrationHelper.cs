using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TossErp.Infrastructure.Database;

/// <summary>
/// Base migration helper providing standard TOSS ERP III migration patterns
/// </summary>
public static class BaseMigrationHelper
{
    /// <summary>
    /// Seeds global reference data during migrations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="data">Data to seed</param>
    public static void SeedGlobalReferenceData<T>(
        MigrationBuilder migrationBuilder, 
        string tableName,
        T[] data) where T : class
    {
        if (data == null || data.Length == 0)
            return;

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.GetGetMethod()?.IsPublic == true)
            .ToArray();
            
        var columns = properties.Select(p => p.Name).ToArray();
        
        var values = new object[data.Length, properties.Length];
        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < properties.Length; j++)
            {
                values[i, j] = properties[j].GetValue(data[i]) ?? DBNull.Value;
            }
        }
        
        migrationBuilder.InsertData(
            table: tableName,
            columns: columns,
            values: values
        );
    }

    /// <summary>
    /// Creates standard tenant isolation index
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="fillFactor">Fill factor (default 90)</param>
    public static void CreateTenantIndex(
        MigrationBuilder migrationBuilder,
        string tableName,
        int fillFactor = 90)
    {
        migrationBuilder.CreateIndex(
            name: $"IX_{tableName}_TenantId",
            table: tableName,
            column: "TenantId")
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());
    }

    /// <summary>
    /// Creates standard audit trail indexes
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="fillFactor">Fill factor (default 80)</param>
    public static void CreateAuditIndexes(
        MigrationBuilder migrationBuilder,
        string tableName,
        int fillFactor = 80)
    {
        migrationBuilder.CreateIndex(
            name: $"IX_{tableName}_CreatedAt",
            table: tableName,
            column: "CreatedAt")
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());

        migrationBuilder.CreateIndex(
            name: $"IX_{tableName}_LastModifiedAt",
            table: tableName,
            column: "LastModifiedAt")
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());
    }

    /// <summary>
    /// Creates standard soft delete index
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="fillFactor">Fill factor (default 95)</param>
    public static void CreateSoftDeleteIndex(
        MigrationBuilder migrationBuilder,
        string tableName,
        int fillFactor = 95)
    {
        migrationBuilder.CreateIndex(
            name: $"IX_{tableName}_IsDeleted",
            table: tableName,
            column: "IsDeleted")
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());
    }

    /// <summary>
    /// Creates composite tenant + business key unique index
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="businessKeyColumn">Business key column name</param>
    /// <param name="fillFactor">Fill factor (default 95)</param>
    public static void CreateTenantBusinessKeyIndex(
        MigrationBuilder migrationBuilder,
        string tableName,
        string businessKeyColumn,
        int fillFactor = 95)
    {
        migrationBuilder.CreateIndex(
            name: $"UX_{tableName}_TenantId_{businessKeyColumn}",
            table: tableName,
            columns: new[] { "TenantId", businessKeyColumn },
            unique: true)
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());
    }

    /// <summary>
    /// Creates composite index for common query patterns
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="columns">Columns to include in index</param>
    /// <param name="fillFactor">Fill factor (default 90)</param>
    /// <param name="includeColumns">Include columns for covering index</param>
    public static void CreateCompositeIndex(
        MigrationBuilder migrationBuilder,
        string tableName,
        string[] columns,
        int fillFactor = 90,
        string[]? includeColumns = null)
    {
        var indexName = $"IX_{tableName}_{string.Join("_", columns)}";
        
        var index = migrationBuilder.CreateIndex(
            name: indexName,
            table: tableName,
            columns: columns)
            .Annotation("Npgsql:StorageParameter:fillfactor", fillFactor.ToString());

        if (includeColumns != null && includeColumns.Length > 0)
        {
            index.Annotation("Npgsql:IncludeProperties", includeColumns);
        }
    }

    /// <summary>
    /// Adds standard check constraints for audit fields
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    public static void AddAuditCheckConstraints(
        MigrationBuilder migrationBuilder,
        string tableName)
    {
        migrationBuilder.Sql($@"
            ALTER TABLE ""{tableName}"" 
            ADD CONSTRAINT ""CK_{tableName}_CreatedBy_NotEmpty"" 
            CHECK (""CreatedBy"" != '');
        ");

        migrationBuilder.Sql($@"
            ALTER TABLE ""{tableName}"" 
            ADD CONSTRAINT ""CK_{tableName}_CreatedAt_Valid"" 
            CHECK (""CreatedAt"" <= ""LastModifiedAt"");
        ");
    }

    /// <summary>
    /// Enables Row Level Security for tenant isolation
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="allowGlobalRead">Allow reading global reference data</param>
    public static void EnableRowLevelSecurity(
        MigrationBuilder migrationBuilder,
        string tableName,
        bool allowGlobalRead = false)
    {
        // Enable RLS
        migrationBuilder.Sql($@"ALTER TABLE ""{tableName}"" ENABLE ROW LEVEL SECURITY;");

        // Create tenant policy for app_service role
        var policyCondition = allowGlobalRead
            ? @"""TenantId"" = '00000000-0000-0000-0000-000000000000'::UUID OR ""TenantId"" = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID)"
            : @"""TenantId"" = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID)";

        migrationBuilder.Sql($@"
            CREATE POLICY ""{tableName}_tenant_policy"" 
            ON ""{tableName}""
            FOR ALL 
            TO app_service
            USING ({policyCondition});
        ");

        // Create read-only policy for app_readonly role
        migrationBuilder.Sql($@"
            CREATE POLICY ""{tableName}_readonly_policy"" 
            ON ""{tableName}""
            FOR SELECT 
            TO app_readonly
            USING ({policyCondition});
        ");

        // Create system policy for migrations and maintenance
        migrationBuilder.Sql($@"
            CREATE POLICY ""{tableName}_system_policy"" 
            ON ""{tableName}""
            FOR ALL 
            TO app_migration
            USING (true);
        ");
    }

    /// <summary>
    /// Disables Row Level Security (for migration rollback)
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    public static void DisableRowLevelSecurity(
        MigrationBuilder migrationBuilder,
        string tableName)
    {
        // Drop policies
        migrationBuilder.Sql($@"DROP POLICY IF EXISTS ""{tableName}_tenant_policy"" ON ""{tableName}"";");
        migrationBuilder.Sql($@"DROP POLICY IF EXISTS ""{tableName}_readonly_policy"" ON ""{tableName}"";");
        migrationBuilder.Sql($@"DROP POLICY IF EXISTS ""{tableName}_system_policy"" ON ""{tableName}"";");

        // Disable RLS
        migrationBuilder.Sql($@"ALTER TABLE ""{tableName}"" DISABLE ROW LEVEL SECURITY;");
    }

    /// <summary>
    /// Removes standard check constraints (for migration rollback)
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    public static void RemoveAuditCheckConstraints(
        MigrationBuilder migrationBuilder,
        string tableName)
    {
        migrationBuilder.Sql($@"ALTER TABLE ""{tableName}"" DROP CONSTRAINT IF EXISTS ""CK_{tableName}_CreatedBy_NotEmpty"";");
        migrationBuilder.Sql($@"ALTER TABLE ""{tableName}"" DROP CONSTRAINT IF EXISTS ""CK_{tableName}_CreatedAt_Valid"";");
    }

    /// <summary>
    /// Drops standard indexes (for migration rollback)
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    /// <param name="tableName">Table name</param>
    /// <param name="businessKeyColumn">Business key column if unique index was created</param>
    public static void DropStandardIndexes(
        MigrationBuilder migrationBuilder,
        string tableName,
        string? businessKeyColumn = null)
    {
        migrationBuilder.DropIndex(name: $"IX_{tableName}_TenantId", table: tableName);
        migrationBuilder.DropIndex(name: $"IX_{tableName}_CreatedAt", table: tableName);
        migrationBuilder.DropIndex(name: $"IX_{tableName}_LastModifiedAt", table: tableName);
        migrationBuilder.DropIndex(name: $"IX_{tableName}_IsDeleted", table: tableName);

        if (!string.IsNullOrEmpty(businessKeyColumn))
        {
            migrationBuilder.DropIndex(name: $"UX_{tableName}_TenantId_{businessKeyColumn}", table: tableName);
        }
    }

    /// <summary>
    /// Creates the standard tenant access predicate function if it doesn't exist
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    public static void CreateTenantAccessFunction(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
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
        ");
    }

    /// <summary>
    /// Drops the tenant access predicate function
    /// </summary>
    /// <param name="migrationBuilder">Migration builder</param>
    public static void DropTenantAccessFunction(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_tenant_access_predicate(UUID);");
    }
}

/// <summary>
/// Standard column definitions for TOSS ERP III entities
/// </summary>
public static class StandardColumns
{
    /// <summary>
    /// Creates standard audit columns configuration
    /// </summary>
    /// <param name="table">Table builder</param>
    public static void AddAuditColumns(ColumnsBuilder table)
    {
        table.Column<DateTimeOffset>("CreatedAt", type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()");
        table.Column<string>("CreatedBy", type: "character varying(100)", maxLength: 100, nullable: false);
        table.Column<DateTimeOffset>("LastModifiedAt", type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()");
        table.Column<string>("LastModifiedBy", type: "character varying(100)", maxLength: 100, nullable: true);
    }

    /// <summary>
    /// Creates standard soft delete columns configuration
    /// </summary>
    /// <param name="table">Table builder</param>
    public static void AddSoftDeleteColumns(ColumnsBuilder table)
    {
        table.Column<bool>("IsDeleted", type: "boolean", nullable: false, defaultValue: false);
        table.Column<DateTimeOffset?>("DeletedAt", type: "timestamp with time zone", nullable: true);
        table.Column<string>("DeletedBy", type: "character varying(100)", maxLength: 100, nullable: true);
    }

    /// <summary>
    /// Creates standard entity columns (Id, TenantId, IsActive)
    /// </summary>
    /// <param name="table">Table builder</param>
    public static void AddStandardEntityColumns(ColumnsBuilder table)
    {
        table.Column<Guid>("Id", type: "uuid", nullable: false);
        table.Column<Guid>("TenantId", type: "uuid", nullable: false);
        table.Column<bool>("IsActive", type: "boolean", nullable: false, defaultValue: true);
    }
}
