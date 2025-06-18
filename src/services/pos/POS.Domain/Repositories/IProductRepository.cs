using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.Enums;

namespace POS.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByStockLevelAsync(int minStockLevel, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetBySearchTermAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<bool> ExistsByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<int> GetTotalStockLevelAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalValueAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasVariantsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasDiscountsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasReviewsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasInventoryMovementsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasPurchaseOrdersAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> HasSalesAsync(Guid productId, CancellationToken cancellationToken = default);
    }
} 
