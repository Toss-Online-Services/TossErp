using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSalesDocumentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesDocuments_Sales_SaleId1",
                table: "SalesDocuments");

            migrationBuilder.DropIndex(
                name: "IX_SalesDocuments_SaleId1",
                table: "SalesDocuments");

            migrationBuilder.DropColumn(
                name: "SaleId1",
                table: "SalesDocuments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SaleId1",
                table: "SalesDocuments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesDocuments_SaleId1",
                table: "SalesDocuments",
                column: "SaleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDocuments_Sales_SaleId1",
                table: "SalesDocuments",
                column: "SaleId1",
                principalTable: "Sales",
                principalColumn: "Id");
        }
    }
}
