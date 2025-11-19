using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstNameLastNameToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIConversations_Shops_ShopId",
                table: "AIConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_AISettings_Shops_ShopId",
                table: "AISettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Shops_ShopId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryStops_Shops_ShopId",
                table: "DeliveryStops");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupBuyPools_Shops_InitiatorShopId",
                table: "GroupBuyPools");

            migrationBuilder.DropForeignKey(
                name: "FK_PayLinks_Shops_ShopId",
                table: "PayLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Shops_ShopId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PoolParticipations_Shops_ShopId",
                table: "PoolParticipations");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Shops_ShopId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Shops_ShopId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Addresses_AddressId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAlerts_Shops_ShopId",
                table: "StockAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Shops_ShopId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Shops_ShopId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreMappings_Shops_StoreId",
                table: "StoreMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shops",
                table: "Shops");

            migrationBuilder.RenameTable(
                name: "Shops",
                newName: "Store");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_OwnerId",
                table: "Store",
                newName: "IX_Store_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_AreaGroup",
                table: "Store",
                newName: "IX_Store_AreaGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Shops_AddressId",
                table: "Store",
                newName: "IX_Store_AddressId");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Store",
                table: "Store",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Store_ShopId",
                table: "AIConversations",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISettings_Store_ShopId",
                table: "AISettings",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Store_ShopId",
                table: "Customers",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryStops_Store_ShopId",
                table: "DeliveryStops",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupBuyPools_Store_InitiatorShopId",
                table: "GroupBuyPools",
                column: "InitiatorShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayLinks_Store_ShopId",
                table: "PayLinks",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Store_ShopId",
                table: "Payments",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoolParticipations_Store_ShopId",
                table: "PoolParticipations",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Store_ShopId",
                table: "PurchaseOrders",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Store_ShopId",
                table: "Receipts",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Store_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAlerts_Store_ShopId",
                table: "StockAlerts",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Store_ShopId",
                table: "StockLevels",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Store_ShopId",
                table: "StockMovements",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Addresses_AddressId",
                table: "Store",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMappings_Store_StoreId",
                table: "StoreMappings",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIConversations_Store_ShopId",
                table: "AIConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_AISettings_Store_ShopId",
                table: "AISettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Store_ShopId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryStops_Store_ShopId",
                table: "DeliveryStops");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupBuyPools_Store_InitiatorShopId",
                table: "GroupBuyPools");

            migrationBuilder.DropForeignKey(
                name: "FK_PayLinks_Store_ShopId",
                table: "PayLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Store_ShopId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PoolParticipations_Store_ShopId",
                table: "PoolParticipations");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Store_ShopId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Store_ShopId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Store_ShopId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAlerts_Store_ShopId",
                table: "StockAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Store_ShopId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Store_ShopId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_Store_Addresses_AddressId",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreMappings_Store_StoreId",
                table: "StoreMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Store",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Store",
                newName: "Shops");

            migrationBuilder.RenameIndex(
                name: "IX_Store_OwnerId",
                table: "Shops",
                newName: "IX_Shops_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Store_AreaGroup",
                table: "Shops",
                newName: "IX_Shops_AreaGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Store_AddressId",
                table: "Shops",
                newName: "IX_Shops_AddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shops",
                table: "Shops",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Shops_ShopId",
                table: "AIConversations",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISettings_Shops_ShopId",
                table: "AISettings",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Shops_ShopId",
                table: "Customers",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryStops_Shops_ShopId",
                table: "DeliveryStops",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupBuyPools_Shops_InitiatorShopId",
                table: "GroupBuyPools",
                column: "InitiatorShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayLinks_Shops_ShopId",
                table: "PayLinks",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Shops_ShopId",
                table: "Payments",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoolParticipations_Shops_ShopId",
                table: "PoolParticipations",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Shops_ShopId",
                table: "PurchaseOrders",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Shops_ShopId",
                table: "Receipts",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Addresses_AddressId",
                table: "Shops",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAlerts_Shops_ShopId",
                table: "StockAlerts",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Shops_ShopId",
                table: "StockLevels",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Shops_ShopId",
                table: "StockMovements",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMappings_Shops_StoreId",
                table: "StoreMappings",
                column: "StoreId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
