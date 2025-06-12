using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseEnumForOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                schema: "POS",
                table: "orders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            // ensure "OrderStatus" column is populated before dropping the "orderstatus" table:
            migrationBuilder.Sql("""
                UPDATE POS.orders 
                SET "OrderStatus" = s."Name"
                FROM POS.orderstatus s
                WHERE s."Id" = orders."OrderStatusId";
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_orders_orderstatus_OrderStatusId",
                schema: "POS",
                table: "orders");

            migrationBuilder.DropTable(
                name: "orderstatus",
                schema: "POS");

            migrationBuilder.DropIndex(
                name: "IX_orders_OrderStatusId",
                schema: "POS",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                schema: "POS",
                table: "orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                schema: "POS",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "orderstatus",
                schema: "POS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderstatus", x => x.Id);
                });

            // ensure "orderstatus" table is seeded & "OrderStatusId" column is populated before dropping the "OrderStatus" column:
            migrationBuilder.Sql("""
                INSERT INTO POS.orderstatus("Id","Name") VALUES
                (1, 'Submitted'),
                (2, 'AwaitingValidation'),
                (3, 'StockConfirmed'),
                (4, 'Paid'),
                (5, 'Shipped'),
                (6, 'Cancelled');

                UPDATE POS.orders
                SET "OrderStatusId" = s."Id"
                FROM POS.orderstatus s
                WHERE s."Name" = orders."OrderStatus";
                """);

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                schema: "POS",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId",
                schema: "POS",
                table: "orders",
                column: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_orderstatus_OrderStatusId",
                schema: "POS",
                table: "orders",
                column: "OrderStatusId",
                principalSchema: "POS",
                principalTable: "orderstatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
