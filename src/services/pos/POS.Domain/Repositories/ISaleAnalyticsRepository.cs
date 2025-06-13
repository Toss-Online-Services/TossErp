using eShop.POS.Domain.AggregatesModel.SaleAggregate;

namespace eShop.POS.Domain.Repositories;

public interface ISaleAnalyticsRepository
{
    Task RecordSaleCompleted(
        string storeId,
        string staffId,
        decimal total,
        bool isOffline,
        DateTime completedAt,
        CancellationToken cancellationToken = default);

    Task RecordSaleRefunded(
        string storeId,
        string staffId,
        decimal amount,
        string reason,
        DateTime refundedAt,
        CancellationToken cancellationToken = default);

    Task RecordPayment(
        string storeId,
        string staffId,
        eShop.POS.Domain.AggregatesModel.SaleAggregate.PaymentMethod method,
        decimal amount,
        DateTime paymentDate,
        CancellationToken cancellationToken = default);

    Task RecordDiscount(
        string storeId,
        string staffId,
        DiscountType type,
        decimal amount,
        DateTime createdAt,
        CancellationToken cancellationToken = default);

    Task RecordSaleSynced(
        string storeId,
        string staffId,
        DateTime syncedAt,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalSalesAsync(
        string storeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalRefundsAsync(
        string storeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IDictionary<eShop.POS.Domain.AggregatesModel.SaleAggregate.PaymentMethod, decimal>> GetPaymentMethodTotalsAsync(
        string storeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<IDictionary<string, decimal>> GetStaffPerformanceAsync(
        string storeId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
} 
