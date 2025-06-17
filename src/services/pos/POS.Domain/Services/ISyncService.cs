using POS.Domain.AggregatesModel.SaleAggregate;

namespace POS.Domain.Services
{
    public interface ISyncService
    {
        Task<bool> IsOnlineAsync();
        Task<bool> SyncSaleAsync(Sale sale);
        Task<bool> SyncPendingSalesAsync();
        Task<bool> SyncInventoryAsync();
        Task<bool> SyncProductsAsync();
        Task<bool> SyncCustomersAsync();
        Task<bool> SyncStaffAsync();
        Task<bool> SyncStoreSettingsAsync();
        Task<bool> SyncAllAsync();
    }
} 
