using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.EventHandlers;

/// <summary>
/// Event handler for PurchaseReceipt events from the Procurement service
/// </summary>
public class PurchaseReceiptEventHandler : INotificationHandler<PurchaseReceiptEvent>
{
    private readonly IPostingRulesService _postingRulesService;
    private readonly ILogger<PurchaseReceiptEventHandler> _logger;

    public PurchaseReceiptEventHandler(
        IPostingRulesService postingRulesService,
        ILogger<PurchaseReceiptEventHandler> logger)
    {
        _postingRulesService = postingRulesService;
        _logger = logger;
    }

    public async Task Handle(PurchaseReceiptEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received PurchaseReceipt event for purchase order {PurchaseOrderId}", notification.PurchaseOrderId);

            // Extract the total amount and tax amount from the purchase receipt
            // In a real implementation, you might need to call the Procurement service to get these details
            var totalAmount = notification.TotalAmount;
            var taxAmount = notification.TaxAmount;
            var tenantId = notification.TenantId;

            await _postingRulesService.HandlePurchaseReceiptAsync(
                notification.PurchaseOrderId,
                totalAmount,
                taxAmount,
                tenantId,
                cancellationToken);

            _logger.LogInformation("Successfully processed PurchaseReceipt event for purchase order {PurchaseOrderId}", notification.PurchaseOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PurchaseReceipt event for purchase order {PurchaseOrderId}", notification.PurchaseOrderId);
            throw;
        }
    }
}

/// <summary>
/// Placeholder event class - in a real implementation, this would come from the Procurement service
/// </summary>
public class PurchaseReceiptEvent : INotification
{
    public Guid PurchaseOrderId { get; }
    public decimal TotalAmount { get; }
    public decimal TaxAmount { get; }
    public string TenantId { get; }
    public DateTime OccurredOn { get; }

    public PurchaseReceiptEvent(Guid purchaseOrderId, decimal totalAmount, decimal taxAmount, string tenantId)
    {
        PurchaseOrderId = purchaseOrderId;
        TotalAmount = totalAmount;
        TaxAmount = taxAmount;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}


