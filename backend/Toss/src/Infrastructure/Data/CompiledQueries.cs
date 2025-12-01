using Microsoft.EntityFrameworkCore;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Infrastructure.Data;

/// <summary>
/// Compiled queries for hot paths to improve performance.
/// EF Core compiles these queries once and reuses them, avoiding query compilation overhead.
/// </summary>
public static class CompiledQueries
{
    /// <summary>
    /// Compiled query for finding a product by barcode within a business context.
    /// Used frequently in POS barcode scanning.
    /// </summary>
    public static readonly Func<ApplicationDbContext, int, string, CancellationToken, Task<Product?>> 
        GetProductByBarcodeAsync = EF.CompileAsyncQuery(
            (ApplicationDbContext context, int businessId, string barcode, CancellationToken ct) =>
                context.Products
                    .Where(p => p.BusinessId == businessId && p.Barcode == barcode && p.IsActive)
                    .FirstOrDefault());

    /// <summary>
    /// Compiled query for finding a product by SKU within a business context.
    /// Used frequently in product lookups.
    /// </summary>
    public static readonly Func<ApplicationDbContext, int, string, CancellationToken, Task<Product?>> 
        GetProductBySkuAsync = EF.CompileAsyncQuery(
            (ApplicationDbContext context, int businessId, string sku, CancellationToken ct) =>
                context.Products
                    .Where(p => p.BusinessId == businessId && p.SKU == sku && p.IsActive)
                    .FirstOrDefault());

    /// <summary>
    /// Compiled query for checking if a sale with a given payment reference already exists.
    /// Used for idempotency checks in POS checkout.
    /// </summary>
    public static readonly Func<ApplicationDbContext, int, string, CancellationToken, Task<Sale?>> 
        GetSaleByPaymentReferenceAsync = EF.CompileAsyncQuery(
            (ApplicationDbContext context, int shopId, string paymentReference, CancellationToken ct) =>
                context.Sales
                    .Where(s => s.ShopId == shopId 
                        && s.SaleType == SaleType.POS
                        && s.Status == SaleStatus.Completed
                        && s.PaymentReference == paymentReference)
                    .FirstOrDefault());

    /// <summary>
    /// Compiled query for getting stock level for a product at a specific shop.
    /// Used frequently in inventory checks during sales.
    /// </summary>
    public static readonly Func<ApplicationDbContext, int, int, CancellationToken, Task<StockLevel?>> 
        GetStockLevelAsync = EF.CompileAsyncQuery(
            (ApplicationDbContext context, int shopId, int productId, CancellationToken ct) =>
                context.StockLevels
                    .Where(sl => sl.ShopId == shopId && sl.ProductId == productId)
                    .FirstOrDefault());
}

