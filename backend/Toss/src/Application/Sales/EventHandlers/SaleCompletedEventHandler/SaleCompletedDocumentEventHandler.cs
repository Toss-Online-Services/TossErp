using MediatR;
using Microsoft.Extensions.Logging;
using Toss.Application.Sales.Commands.CreateSalesDocument;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Automatically creates sales documents (receipts or invoices) based on payment method and customer presence.
/// </summary>
public class SaleCompletedDocumentEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<SaleCompletedDocumentEventHandler> _logger;

    // Payment methods that warrant immediate receipt generation
    private static readonly PaymentType[] ImmediatePaymentMethods =
    {
        PaymentType.Cash,
        PaymentType.Card,
        PaymentType.MobileMoney
    };

    // Payment methods that require invoice generation (credit-like)
    private static readonly PaymentType[] CreditPaymentMethods =
    {
        PaymentType.BankTransfer,
        PaymentType.PayLink
    };

    public SaleCompletedDocumentEventHandler(
        ISender sender,
        ILogger<SaleCompletedDocumentEventHandler> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        var sale = notification.Sale;

        // Handle immediate payment methods - create receipt
        if (ImmediatePaymentMethods.Contains(sale.PaymentMethod))
        {
            await CreateReceipt(sale, cancellationToken);
            return;
        }

        // Handle credit payment methods - create invoice if customer exists
        if (CreditPaymentMethods.Contains(sale.PaymentMethod))
        {
            if (!sale.CustomerId.HasValue)
            {
                _logger.LogDebug(
                    "Sale {SaleNumber} uses payment method {PaymentMethod} but has no customer - skipping invoice creation",
                    sale.SaleNumber,
                    sale.PaymentMethod);
                return;
            }

            await CreateInvoice(sale, cancellationToken);
            return;
        }

        _logger.LogDebug(
            "Sale {SaleNumber} uses payment method {PaymentMethod} - no document auto-creation configured",
            sale.SaleNumber,
            sale.PaymentMethod);
    }

    private async Task CreateReceipt(Sale sale, CancellationToken cancellationToken)
    {
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

    private async Task CreateInvoice(Sale sale, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Auto-creating invoice for sale {SaleNumber} with customer {CustomerId} and {PaymentMethod} payment",
            sale.SaleNumber,
            sale.CustomerId!.Value,
            sale.PaymentMethod);

        try
        {
            // Create invoice document (idempotent)
            await _sender.Send(new CreateSalesDocumentCommand
            {
                SaleId = sale.Id,
                DocumentType = SalesDocumentType.Invoice,
                Notes = $"Auto-generated for {sale.PaymentMethod} payment"
            }, cancellationToken);

            _logger.LogInformation(
                "Successfully auto-created invoice for sale {SaleNumber}",
                sale.SaleNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to auto-create invoice for sale {SaleNumber}",
                sale.SaleNumber);
            // Don't throw - invoice creation failure shouldn't block sale completion
        }
    }
}

