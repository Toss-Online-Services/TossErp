using Microsoft.Extensions.Logging;
using eShop.POS.Infrastructure.Repositories;

namespace eShop.POS.API.Services;

public class SyncService : ISyncService
{
    private readonly ILogger<SyncService> _logger;
    private readonly ISaleRepository _saleRepository;
    private readonly ISyncLogRepository _syncLogRepository;

    public SyncService(
        ILogger<SyncService> logger,
        ISaleRepository saleRepository,
        ISyncLogRepository syncLogRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _syncLogRepository = syncLogRepository;
    }

    public async Task MarkSaleAsSynced(int saleId, DateTime syncedAt, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking sale {SaleId} as synced", saleId);

        try
        {
            var sale = await _saleRepository.GetAsync(saleId);
            if (sale == null)
                throw new InvalidOperationException($"Sale {saleId} not found");

            sale.MarkAsSynced();
            await _saleRepository.UpdateAsync(sale);

            // Record sync in log
            await _syncLogRepository.RecordSync(
                saleId,
                sale.StoreId,
                syncedAt,
                cancellationToken);

            _logger.LogInformation("Successfully marked sale {SaleId} as synced", saleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking sale {SaleId} as synced", saleId);
            throw;
        }
    }
} 
