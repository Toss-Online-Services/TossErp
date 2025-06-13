using Microsoft.Extensions.Logging;
using POS.Infrastructure.Repositories;

namespace POS.API.Services;

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    public InventoryService(
        ILogger<InventoryService> logger,
        ISaleRepository saleRepository,
        IProductRepository productRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    public async Task UpdateStockForSale(int saleId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating stock for sale {SaleId}", saleId);

        try
        {
            var sale = await _saleRepository.GetAsync(saleId);
            if (sale == null)
                throw new InvalidOperationException($"Sale {saleId} not found");

            foreach (var item in sale.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for sale {SaleId}", item.ProductId, saleId);
                    continue;
                }

                product.UpdateStock(-item.Quantity);
                await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Updated stock for product {ProductId} in sale {SaleId}", item.ProductId, saleId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for sale {SaleId}", saleId);
            throw;
        }
    }

    public async Task RestoreStockForRefund(int saleId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Restoring stock for refunded sale {SaleId}", saleId);

        try
        {
            var sale = await _saleRepository.GetAsync(saleId);
            if (sale == null)
                throw new InvalidOperationException($"Sale {saleId} not found");

            foreach (var item in sale.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for refunded sale {SaleId}", item.ProductId, saleId);
                    continue;
                }

                product.UpdateStock(item.Quantity);
                await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Restored stock for product {ProductId} in refunded sale {SaleId}", item.ProductId, saleId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring stock for refunded sale {SaleId}", saleId);
            throw;
        }
    }
} 
