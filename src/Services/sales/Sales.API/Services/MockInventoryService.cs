using TossErp.Sales.Application.Common.Interfaces;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Mock implementation of IInventoryService for MVP
/// </summary>
public class MockInventoryService : IInventoryService
{
    private readonly ILogger<MockInventoryService> _logger;

    public MockInventoryService(ILogger<MockInventoryService> logger)
    {
        _logger = logger;
    }

    public Task UpdateStockLevelsAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default)
    {
        var itemsList = items.ToList();
        _logger.LogInformation("Mock inventory update for sale {SaleId} with {ItemCount} items", 
            saleId, itemsList.Count);
        
        foreach (var item in itemsList)
        {
            _logger.LogInformation("Mock inventory update: Item {ItemId} ({ItemName}) - Quantity: {Quantity}", 
                item.ItemId, item.ItemName, item.Quantity);
        }
        
        return Task.CompletedTask;
    }

    public Task ReserveStockAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default)
    {
        var itemsList = items.ToList();
        _logger.LogInformation("Mock stock reservation for sale {SaleId} with {ItemCount} items", 
            saleId, itemsList.Count);
        
        foreach (var item in itemsList)
        {
            _logger.LogInformation("Mock stock reservation: Item {ItemId} ({ItemName}) - Quantity: {Quantity}", 
                item.ItemId, item.ItemName, item.Quantity);
        }
        
        return Task.CompletedTask;
    }

    public Task ReleaseReservedStockAsync(Guid saleId, IEnumerable<SaleItemDto> items, CancellationToken cancellationToken = default)
    {
        var itemsList = items.ToList();
        _logger.LogInformation("Mock stock release for sale {SaleId} with {ItemCount} items", 
            saleId, itemsList.Count);
        
        foreach (var item in itemsList)
        {
            _logger.LogInformation("Mock stock release: Item {ItemId} ({ItemName}) - Quantity: {Quantity}", 
                item.ItemId, item.ItemName, item.Quantity);
        }
        
        return Task.CompletedTask;
    }
}
