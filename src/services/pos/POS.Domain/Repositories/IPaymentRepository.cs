using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.Enums;

namespace POS.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByPaymentMethodAsync(PaymentType paymentMethod, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(POS.Domain.AggregatesModel.PaymentAggregate.PaymentStatus status, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByPaymentMethodAsync(PaymentType paymentMethod, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaymentsByStatusAsync(POS.Domain.AggregatesModel.PaymentAggregate.PaymentStatus status, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<bool> HasRefundAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<bool> HasPartialRefundAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<bool> HasSplitPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<bool> HasReconciliationAsync(Guid paymentId, CancellationToken cancellationToken = default);
} 
