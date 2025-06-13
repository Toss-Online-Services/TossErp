namespace POS.API.Services;

public interface IInventoryService
{
    Task UpdateStockForSale(int saleId, CancellationToken cancellationToken = default);
    Task RestoreStockForRefund(int saleId, CancellationToken cancellationToken = default);
} 
