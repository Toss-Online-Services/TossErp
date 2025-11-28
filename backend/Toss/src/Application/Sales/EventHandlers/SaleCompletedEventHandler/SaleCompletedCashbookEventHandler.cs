using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Accounting.Commands.RecordCashIn;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Payments.Commands.RecordPayment;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Toss.Domain.Events;

namespace Toss.Application.Sales.EventHandlers;

/// <summary>
/// Handles cashbook entry creation and payment recording for completed POS sales.
/// </summary>
public class SaleCompletedCashbookEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly ISender _sender;
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ILogger<SaleCompletedCashbookEventHandler> _logger;

    public SaleCompletedCashbookEventHandler(
        ISender sender,
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ILogger<SaleCompletedCashbookEventHandler> logger)
    {
        _sender = sender;
        _context = context;
        _businessContext = businessContext;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        var sale = notification.Sale;

        // Only process POS sales that are completed and have immediate payment methods
        if (sale.SaleType != SaleType.POS || sale.Status != SaleStatus.Completed)
        {
            return;
        }

        // Skip if payment method doesn't require immediate cashbook entry
        // BankTransfer and PayLink are handled via invoice payment flow
        if (sale.PaymentMethod == PaymentType.BankTransfer || sale.PaymentMethod == PaymentType.PayLink)
        {
            return;
        }

        try
        {
            // Record payment first
            var paymentId = await _sender.Send(new RecordPaymentCommand
            {
                ShopId = sale.ShopId,
                SaleId = sale.Id,
                Amount = sale.Total,
                PaymentType = sale.PaymentMethod,
                TransactionRef = sale.PaymentReference,
                Notes = $"POS sale {sale.SaleNumber}"
            }, cancellationToken);

            _logger.LogInformation(
                "Recorded payment {PaymentId} for POS sale {SaleNumber}",
                paymentId,
                sale.SaleNumber);

            // Get default cash account for the shop's business
            var accountId = await GetDefaultCashAccountAsync(sale.ShopId, cancellationToken);

            if (accountId.HasValue)
            {
                // Record cashbook entry
                await _sender.Send(new RecordCashInCommand
                {
                    AccountId = accountId.Value,
                    Amount = sale.Total,
                    EntryDate = sale.SaleDate,
                    Reference = sale.SaleNumber,
                    Notes = $"POS sale {sale.SaleNumber}",
                    SourceType = "Sale",
                    SourceId = sale.Id,
                    PaymentId = paymentId
                }, cancellationToken);

                _logger.LogInformation(
                    "Recorded cashbook entry for POS sale {SaleNumber} to account {AccountId}",
                    sale.SaleNumber,
                    accountId.Value);
            }
            else
            {
                _logger.LogWarning(
                    "No default cash account found for shop {ShopId} - cashbook entry not created for sale {SaleNumber}",
                    sale.ShopId,
                    sale.SaleNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to record cashbook entry for POS sale {SaleNumber}",
                sale.SaleNumber);
            // Don't throw - cashbook entry failure shouldn't block sale completion
        }
    }

    private async Task<int?> GetDefaultCashAccountAsync(int shopId, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            return null;
        }

        // Get the shop to find its business
        var shop = await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == shopId, cancellationToken);

        if (shop == null)
        {
            return null;
        }

        // Get the first active cash account for the business
        var account = await _context.Accounts
            .Where(a => a.BusinessId == shop.BusinessId 
                && a.Type == AccountType.Cash 
                && a.IsActive)
            .OrderBy(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return account?.Id;
    }
}

