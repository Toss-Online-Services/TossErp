using eShop.POS.Domain.AggregatesModel.SaleAggregate;

namespace eShop.POS.API.Services;

public interface IAnalyticsService
{
    Task TrackSaleCompleted(string storeId, string staffId, decimal total, bool isOffline, CancellationToken cancellationToken = default);
    Task TrackSaleRefunded(string storeId, string staffId, decimal amount, string reason, CancellationToken cancellationToken = default);
    Task TrackPaymentAdded(string storeId, string staffId, PaymentMethod method, decimal amount, CancellationToken cancellationToken = default);
    Task TrackDiscountAdded(string storeId, string staffId, DiscountType type, decimal amount, CancellationToken cancellationToken = default);
    Task TrackSaleSynced(string storeId, string staffId, DateTime syncedAt, CancellationToken cancellationToken = default);
} 
