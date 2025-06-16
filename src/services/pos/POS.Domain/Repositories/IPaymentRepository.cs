using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetBySaleAsync(Guid saleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, Guid storeId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByDateRangeAsync(Guid storeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
} 
