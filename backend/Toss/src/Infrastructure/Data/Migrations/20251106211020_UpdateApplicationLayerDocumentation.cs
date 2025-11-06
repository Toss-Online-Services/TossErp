using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationLayerDocumentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_PurchaseDocuments_Store_ShopId",
                table: "PurchaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Store_ShopId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Store_ShopId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesDocuments_Store_ShopId",
                table: "SalesDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Store_ShopId",
                table: "ShoppingCartItems");

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

            migrationBuilder.RenameTable(
                name: "Store",
                newName: "Stores");

            migrationBuilder.RenameIndex(
                name: "IX_Store_OwnerId",
                table: "Stores",
                newName: "IX_Stores_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Store_AreaGroup",
                table: "Stores",
                newName: "IX_Stores_AreaGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Store_AddressId",
                table: "Stores",
                newName: "IX_Stores_AddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stores",
                table: "Stores",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Stores_ShopId",
                table: "AIConversations",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISettings_Stores_ShopId",
                table: "AISettings",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Stores_ShopId",
                table: "Customers",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryStops_Stores_ShopId",
                table: "DeliveryStops",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupBuyPools_Stores_InitiatorShopId",
                table: "GroupBuyPools",
                column: "InitiatorShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PayLinks_Stores_ShopId",
                table: "PayLinks",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Stores_ShopId",
                table: "Payments",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoolParticipations_Stores_ShopId",
                table: "PoolParticipations",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDocuments_Stores_ShopId",
                table: "PurchaseDocuments",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Stores_ShopId",
                table: "PurchaseOrders",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Stores_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDocuments_Stores_ShopId",
                table: "SalesDocuments",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Stores_ShopId",
                table: "ShoppingCartItems",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAlerts_Stores_ShopId",
                table: "StockAlerts",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Stores_ShopId",
                table: "StockLevels",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Stores_ShopId",
                table: "StockMovements",
                column: "ShopId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreMappings_Stores_StoreId",
                table: "StoreMappings",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Addresses_AddressId",
                table: "Stores",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIConversations_Stores_ShopId",
                table: "AIConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_AISettings_Stores_ShopId",
                table: "AISettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Stores_ShopId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryStops_Stores_ShopId",
                table: "DeliveryStops");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupBuyPools_Stores_InitiatorShopId",
                table: "GroupBuyPools");

            migrationBuilder.DropForeignKey(
                name: "FK_PayLinks_Stores_ShopId",
                table: "PayLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Stores_ShopId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PoolParticipations_Stores_ShopId",
                table: "PoolParticipations");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDocuments_Stores_ShopId",
                table: "PurchaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Stores_ShopId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Stores_ShopId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesDocuments_Stores_ShopId",
                table: "SalesDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Stores_ShopId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAlerts_Stores_ShopId",
                table: "StockAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Stores_ShopId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Stores_ShopId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreMappings_Stores_StoreId",
                table: "StoreMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Addresses_AddressId",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stores",
                table: "Stores");

            migrationBuilder.RenameTable(
                name: "Stores",
                newName: "Store");

            migrationBuilder.RenameIndex(
                name: "IX_Stores_OwnerId",
                table: "Store",
                newName: "IX_Store_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Stores_AreaGroup",
                table: "Store",
                newName: "IX_Store_AreaGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Stores_AddressId",
                table: "Store",
                newName: "IX_Store_AddressId");

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
                name: "FK_PurchaseDocuments_Store_ShopId",
                table: "PurchaseDocuments",
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
                name: "FK_Sales_Store_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDocuments_Store_ShopId",
                table: "SalesDocuments",
                column: "ShopId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Store_ShopId",
                table: "ShoppingCartItems",
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
    }
}
