using Microsoft.Extensions.Logging;
using eShop.POS.Domain.AggregatesModel.SaleAggregate;
using eShop.POS.Infrastructure.Repositories;

namespace eShop.POS.API.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly ISaleAnalyticsRepository _analyticsRepository;

    public AnalyticsService(
        ILogger<AnalyticsService> logger,
        ISaleAnalyticsRepository analyticsRepository)
    {
        _logger = logger;
        _analyticsRepository = analyticsRepository;
    }

    public async Task TrackSaleCompleted(string storeId, string staffId, decimal total, bool isOffline, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tracking completed sale for store {StoreId}, staff {StaffId}", storeId, staffId);

        try
        {
            await _analyticsRepository.RecordSaleCompleted(
                storeId,
                staffId,
                total,
                isOffline,
                DateTime.UtcNow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking completed sale for store {StoreId}, staff {StaffId}", storeId, staffId);
            throw;
        }
    }

    public async Task TrackSaleRefunded(string storeId, string staffId, decimal amount, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tracking refunded sale for store {StoreId}, staff {StaffId}", storeId, staffId);

        try
        {
            await _analyticsRepository.RecordSaleRefunded(
                storeId,
                staffId,
                amount,
                reason,
                DateTime.UtcNow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking refunded sale for store {StoreId}, staff {StaffId}", storeId, staffId);
            throw;
        }
    }

    public async Task TrackPaymentAdded(string storeId, string staffId, PaymentMethod method, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tracking payment for store {StoreId}, staff {StaffId}", storeId, staffId);

        try
        {
            await _analyticsRepository.RecordPayment(
                storeId,
                staffId,
                method,
                amount,
                DateTime.UtcNow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking payment for store {StoreId}, staff {StaffId}", storeId, staffId);
            throw;
        }
    }

    public async Task TrackDiscountAdded(string storeId, string staffId, DiscountType type, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tracking discount for store {StoreId}, staff {StaffId}", storeId, staffId);

        try
        {
            await _analyticsRepository.RecordDiscount(
                storeId,
                staffId,
                type,
                amount,
                DateTime.UtcNow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking discount for store {StoreId}, staff {StaffId}", storeId, staffId);
            throw;
        }
    }

    public async Task TrackSaleSynced(string storeId, string staffId, DateTime syncedAt, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tracking synced sale for store {StoreId}, staff {StaffId}", storeId, staffId);

        try
        {
            await _analyticsRepository.RecordSaleSynced(
                storeId,
                staffId,
                syncedAt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking synced sale for store {StoreId}, staff {StaffId}", storeId, staffId);
            throw;
        }
    }
} 
