using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Repositories;

namespace POS.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStoreAsync(string storeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
} 
