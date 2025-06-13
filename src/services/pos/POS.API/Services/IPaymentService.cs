using eShop.POS.Domain.AggregatesModel.SaleAggregate;

namespace eShop.POS.API.Services;

public interface IPaymentService
{
    Task ProcessPayment(
        int saleId,
        PaymentMethod method,
        decimal amount,
        string reference = null,
        string cardLast4 = null,
        string cardType = null,
        CancellationToken cancellationToken = default);

    Task ProcessRefund(
        int originalSaleId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default);
} 
