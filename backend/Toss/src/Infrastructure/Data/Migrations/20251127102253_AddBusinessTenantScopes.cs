using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessTenantScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vendors_Name",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_Email",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_IsActive_IsAvailable",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Vendors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BusinessId",
                table: "Drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBusinesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBusinesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBusinesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBusinesses_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessId",
                table: "Vendors",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BusinessId_Name",
                table: "Vendors",
                columns: new[] { "BusinessId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BusinessId",
                table: "Stores",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BusinessId",
                table: "Products",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BusinessId_SKU",
                table: "Products",
                columns: new[] { "BusinessId", "SKU" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_BusinessId_Email",
                table: "Drivers",
                columns: new[] { "BusinessId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_BusinessId_IsActive_IsAvailable",
                table: "Drivers",
                columns: new[] { "BusinessId", "IsActive", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Code",
                table: "Businesses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBusinesses_BusinessId",
                table: "UserBusinesses",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBusinesses_User_Default",
                table: "UserBusinesses",
                column: "UserId",
                unique: true,
                filter: "\"IsDefault\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_UserBusinesses_UserId_BusinessId",
                table: "UserBusinesses",
                columns: new[] { "UserId", "BusinessId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Businesses_BusinessId",
                table: "Drivers",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Businesses_BusinessId",
                table: "Products",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Businesses_BusinessId",
                table: "Stores",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Businesses_BusinessId",
                table: "Vendors",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Businesses_BusinessId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Businesses_BusinessId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Businesses_BusinessId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Businesses_BusinessId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "UserBusinesses");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_BusinessId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_BusinessId_Name",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Stores_BusinessId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Products_BusinessId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_BusinessId_SKU",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_BusinessId_Email",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_BusinessId_IsActive_IsAvailable",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Drivers");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name",
                table: "Vendors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Email",
                table: "Drivers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IsActive_IsAvailable",
                table: "Drivers",
                columns: new[] { "IsActive", "IsAvailable" });
        }
    }
}
