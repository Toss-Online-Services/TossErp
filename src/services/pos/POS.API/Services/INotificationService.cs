namespace eShop.POS.API.Services;

public interface INotificationService
{
    Task NotifyHighValueSale(string storeId, int saleId, decimal total, CancellationToken cancellationToken = default);
} 
