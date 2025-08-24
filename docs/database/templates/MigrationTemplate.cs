# Entity Framework Migration Template

This is a template for creating EF Core migrations that follow TOSS ERP III standards.

## Instructions

1. Copy this template to your service's Migrations folder
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

            // Example child table with foreign key
            migrationBuilder.CreateTable(
                name: "[EntityName]Details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    [EntityName]Id = table.Column<Guid>(type: "uuid", nullable: false),
                    
                    // Business columns
                    DetailName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    
                    // Audit Trail
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_[EntityName]Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_[EntityName]Details_[EntityName]s_[EntityName]Id",
                        column: x => x.[EntityName]Id,
                        principalTable: "[EntityName]s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]Details_TenantId",
                table: "[EntityName]Details",
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

            // Foreign key indexes
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]Details_[EntityName]Id",
                table: "[EntityName]Details",
                column: "[EntityName]Id")
                .Annotation("Npgsql:StorageParameter:fillfactor", "90");

            // Audit trail indexes for reporting
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_CreatedAt",
                table: "[EntityName]s",
                column: "CreatedAt")
                .Annotation("Npgsql:StorageParameter:fillfactor", "80");

            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_LastModifiedAt",
                table: "[EntityName]s",
                column: "LastModifiedAt")
                .Annotation("Npgsql:StorageParameter:fillfactor", "80");

            // Soft delete index (if applicable)
            migrationBuilder.CreateIndex(
                name: "IX_[EntityName]s_IsDeleted",
                table: "[EntityName]s",
                column: "IsDeleted")
                .Annotation("Npgsql:StorageParameter:fillfactor", "95");
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

            // Business rule constraints
            migrationBuilder.Sql(@"
                ALTER TABLE ""[EntityName]Details"" 
                ADD CONSTRAINT ""CK_[EntityName]Details_Order_Positive"" 
                CHECK (""Order"" >= 0);
            ");

            // Audit trail constraints
            migrationBuilder.Sql(@"
                ALTER TABLE ""[EntityName]s"" 
                ADD CONSTRAINT ""CK_[EntityName]s_CreatedBy_NotEmpty"" 
                CHECK (""CreatedBy"" != '');
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
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]Details"" ENABLE ROW LEVEL SECURITY;");

            // Create security policies for tenant isolation
            migrationBuilder.Sql(@"
                CREATE POLICY ""[EntityName]s_tenant_policy"" 
                ON ""[EntityName]s""
                FOR ALL 
                TO app_service
                USING (""TenantId"" = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID));
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY ""[EntityName]Details_tenant_policy"" 
                ON ""[EntityName]Details""
                FOR ALL 
                TO app_service
                USING (""TenantId"" = CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID));
            ");

            // Create policies for system operations (no tenant filter)
            migrationBuilder.Sql(@"
                CREATE POLICY ""[EntityName]s_system_policy"" 
                ON ""[EntityName]s""
                FOR ALL 
                TO app_migration
                USING (true);
            ");

            migrationBuilder.Sql(@"
                CREATE POLICY ""[EntityName]Details_system_policy"" 
                ON ""[EntityName]Details""
                FOR ALL 
                TO app_migration
                USING (true);
            ");
        }

        private void DisableRowLevelSecurity(MigrationBuilder migrationBuilder)
        {
            // Drop security policies
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""[EntityName]s_tenant_policy"" ON ""[EntityName]s"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""[EntityName]Details_tenant_policy"" ON ""[EntityName]Details"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""[EntityName]s_system_policy"" ON ""[EntityName]s"";");
            migrationBuilder.Sql(@"DROP POLICY IF EXISTS ""[EntityName]Details_system_policy"" ON ""[EntityName]Details"";");

            // Disable Row Level Security
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DISABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]Details"" DISABLE ROW LEVEL SECURITY;");
        }

        private void RemoveConstraints(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]s_Code_Length"";");
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]s_Name_Length"";");
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]Details"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]Details_Order_Positive"";");
            migrationBuilder.Sql(@"ALTER TABLE ""[EntityName]s"" DROP CONSTRAINT IF EXISTS ""CK_[EntityName]s_CreatedBy_NotEmpty"";");
        }

        private void DropIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_TenantId", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]Details_TenantId", table: "[EntityName]Details");
            migrationBuilder.DropIndex(name: "UX_[EntityName]s_TenantId_Code", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_TenantId_Status_IsActive", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]Details_[EntityName]Id", table: "[EntityName]Details");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_CreatedAt", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_LastModifiedAt", table: "[EntityName]s");
            migrationBuilder.DropIndex(name: "IX_[EntityName]s_IsDeleted", table: "[EntityName]s");
        }

        private void DropTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "[EntityName]Details");
            migrationBuilder.DropTable(name: "[EntityName]s");
        }
    }
}
