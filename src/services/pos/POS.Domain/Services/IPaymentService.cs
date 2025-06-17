using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Enums;

namespace POS.Domain.Services
{
    public interface IPaymentService
    {
        Task<bool> IsOnlineAsync();
        Task<bool> CanProcessOfflinePaymentAsync(PaymentType paymentType, decimal amount);
        Task<bool> ProcessPaymentAsync(Payment payment, bool isOffline = false);
        Task<bool> ProcessRefundAsync(Payment payment, decimal amount, string reason);
        Task<bool> SettleOfflinePaymentsAsync();
        Task<bool> ValidateOfflinePaymentLimitAsync(PaymentType paymentType, decimal amount);
    }
} 
