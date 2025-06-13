namespace eShop.POS.API.Services;

public interface ISyncService
{
    Task MarkSaleAsSynced(int saleId, DateTime syncedAt, CancellationToken cancellationToken = default);
} 
