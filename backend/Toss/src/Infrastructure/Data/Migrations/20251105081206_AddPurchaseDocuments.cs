using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: true),
                    DocumentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaidDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsMatchedToPO = table.Column<bool>(type: "boolean", nullable: false),
                    IsMatchedToReceipt = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDocuments_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDocuments_Store_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDocuments_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_DocumentDate",
                table: "PurchaseDocuments",
                column: "DocumentDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_DocumentNumber",
                table: "PurchaseDocuments",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_DocumentType",
                table: "PurchaseDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_IsApproved",
                table: "PurchaseDocuments",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_IsMatchedToPO_IsMatchedToReceipt",
                table: "PurchaseDocuments",
                columns: new[] { "IsMatchedToPO", "IsMatchedToReceipt" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_IsPaid",
                table: "PurchaseDocuments",
                column: "IsPaid");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_PurchaseOrderId",
                table: "PurchaseDocuments",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_ShopId",
                table: "PurchaseDocuments",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDocuments_VendorId",
                table: "PurchaseDocuments",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseDocuments");
        }
    }
}
