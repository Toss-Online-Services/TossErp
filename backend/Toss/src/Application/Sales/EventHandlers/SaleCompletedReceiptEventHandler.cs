using Microsoft.Extensions.Logging;
using Toss.Application.Sales.Commands.CreateSalesDocument;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Automatically creates a receipt document for sales with immediate payment methods.
/// </summary>
public class SaleCompletedReceiptEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<SaleCompletedReceiptEventHandler> _logger;

    // Payment methods that warrant immediate receipt generation
    private static readonly PaymentType[] ImmediatePaymentMethods = 
    {
        PaymentType.Cash,
        PaymentType.Card,
        PaymentType.MobileMoney
    };

    public SaleCompletedReceiptEventHandler(
        ISender sender,
        ILogger<SaleCompletedReceiptEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        var sale = notification.Sale;

        // Only auto-create receipts for immediate payment methods
        if (!ImmediatePaymentMethods.Contains(sale.PaymentMethod))
        {
            _logger.LogDebug(
                "Sale {SaleNumber} uses payment method {PaymentMethod} - skipping auto-receipt creation",
                sale.SaleNumber,
                sale.PaymentMethod);
            return;
        }

        _logger.LogInformation(
            "Auto-creating receipt for sale {SaleNumber} with {PaymentMethod} payment",
            sale.SaleNumber,
            sale.PaymentMethod);

        try
        {
            // Create unified receipt document (idempotent)
            await _sender.Send(new CreateSalesDocumentCommand
            {
                SaleId = sale.Id,
                DocumentType = SalesDocumentType.Receipt,
                Notes = $"Auto-generated for {sale.PaymentMethod} payment"
            }, cancellationToken);

            _logger.LogInformation(
                "Successfully auto-created receipt for sale {SaleNumber}",
                sale.SaleNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to auto-create receipt for sale {SaleNumber}",
                sale.SaleNumber);
            // Don't throw - receipt creation failure shouldn't block sale completion
        }
    }
}
