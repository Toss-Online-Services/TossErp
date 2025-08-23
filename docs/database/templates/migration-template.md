# Entity Framework Migration Template

This template demonstrates the standard structure for EF Core migrations in TOSS ERP III.

## Instructions

1. Copy this template when creating new migrations
2. Replace the following placeholders:
   - `[ServiceName]` with actual service name (e.g., Assets, Stock, Sales)
   - `[EntityName]` with actual entity name (e.g., Asset, Item, Order)
   - `[MigrationName]` with your migration name
   - `[Description]` with migration description

## Template Code

```csharp
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace [ServiceName].[ServiceName].Infrastructure.Migrations
{
    /// <summary>
    /// [Description]
    /// Replace placeholders with actual values
    /// </summary>
    public partial class [MigrationName] : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create tables with standard structure
            CreateTables(migrationBuilder);
            
            // Step 2: Create performance indexes
            CreateIndexes(migrationBuilder);
            
            // Step 3: Add constraints and validations
            AddConstraints(migrationBuilder);
            
            // Step 4: Seed reference data
            SeedReferenceData(migrationBuilder);
            
            // Step 5: Enable Row Level Security
            EnableRowLevelSecurity(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse operations in opposite order
            DisableRowLevelSecurity(migrationBuilder);
            RemoveConstraints(migrationBuilder);
            DropIndexes(migrationBuilder);
            DropTables(migrationBuilder);
        }

        private void CreateTables(MigrationBuilder migrationBuilder)
        {
            // Example table creation following TOSS standards
            migrationBuilder.CreateTable(
                name: "[EntityName]s",
                columns: table => new
                {
                    // Primary Key
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    
                    // Multi-tenancy (Required)
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    
                    // Business columns - customize as needed
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    
                    // Status/State
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    
                    // Audit Trail (Required)
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    
                    // Soft Delete (Optional)
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_[EntityName]s", x => x.Id);
                });
        }

        private void CreateIndexes(MigrationBuilder migrationBuilder)
        {
            // Tenant isolation indexes (Required for all tables)
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_TenantId",
                table: "[EntityName]s",
                column: "TenantId")
                .Annotation("Npgsql:StorageParameter:fillfactor", "90");

            // Business unique constraints
            migrationBuilder.CreateIndex(
                name: "UX_[EntityName]s_TenantId_Code",
                table: "[EntityName]s",
                columns: new[] { "TenantId", "Code" },
                unique: true)
                .Annotation("Npgsql:StorageParameter:fillfactor", "95");

            // Composite indexes for common query patterns
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_TenantId_Status_IsActive",
                table: "[EntityName]s",
                columns: new[] { "TenantId", "Status", "IsActive" })
                .Annotation("Npgsql:StorageParameter:fillfactor", "90");

            // Audit trail indexes for reporting
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_CreatedAt",
                table: "[EntityName]s",
                column: "CreatedAt")
                .Annotation("Npgsql:StorageParameter:fillfactor", "80");
        }

        private void AddConstraints(MigrationBuilder migrationBuilder)
        {
            // Check constraints for data integrity
            migrationBuilder.Sql(@"
                ALTER TABLE ""[EntityName]s"" 
                ADD CONSTRAINT ""CK_[EntityName]s_Code_Length"" 
                CHECK (char_length(""Code"") >= 2);
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""[EntityName]s"" 
                ADD CONSTRAINT ""CK_[EntityName]s_Name_Length"" 
                CHECK (char_length(""Name"") >= 1);
            ");
        }

        private void SeedReferenceData(MigrationBuilder migrationBuilder)
        {
            // Seed global reference data (TenantId = '00000000-0000-0000-0000-000000000000')
            migrationBuilder.InsertData(
                table: "[EntityName]s",
                columns: new[] { "Id", "TenantId", "Name", "Code", "Description", "Status", "IsActive", "CreatedAt", "CreatedBy", "LastModifiedAt" },
                values: new object[,]
                {
                    {
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Guid.Empty, // Global template
                        "Default [EntityName]",
                        "DEFAULT",
                        "Default [EntityName] template for new tenants",
                        "Active",
                        true,
                        new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                        "System",
                        new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
                    }
                });
        }

        private void EnableRowLevelSecurity(MigrationBuilder migrationBuilder)
        {
            // Enable Row Level Security on tables
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" ENABLE ROW LEVEL SECURITY;");

            // Create security policies for tenant isolation
            migrationBuilder.Sql(@"
                CREATE POLICY ""[EntityName]s_tenant_policy"" 
                ON ""[EntityName]s""
                FOR ALL 
                TO app_service
                USING (""TenantId"" = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID));
            ");
        }

        private void DisableRowLevelSecurity(MigrationBuilder migrationBuilder)
        {
            // Drop security policies
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""[EntityName]s_tenant_policy"" ON ""[EntityName]s"";");

            // Disable Row Level Security
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DISABLE ROW LEVEL SECURITY;");
        }

        private void RemoveConstraints(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]s_Code_Length"";");
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]s_Name_Length"";");
        }

        private void DropIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_TenantId", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "UX_[EntityName]s_TenantId_Code", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_TenantId_Status_IsActive", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_CreatedAt", table: "[EntityName]s");
        }

        private void DropTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "[EntityName]s");
        }
    }
}
```

## Usage Example

When creating a migration for Assets service:

1. Replace `[ServiceName]` with `Assets`
2. Replace `[EntityName]` with `Asset`
3. Replace `[MigrationName]` with `InitialAssetSchema`
4. Replace `[Description]` with `Initial schema for Asset management`

This will generate a properly structured migration following all TOSS ERP III standards.
