using TossErp.Accounting.Domain.Common;

namespace TossErp.Accounting.Application.Common.Interfaces;

/// <summary>
/// Service for handling posting rules and automatic accounting entries
/// </summary>
public interface IPostingRulesService
{
    /// <summary>
    /// Handle sale completed event and create corresponding accounting entries
    /// </summary>
    Task HandleSaleCompletedAsync(Guid saleId, decimal totalAmount, decimal taxAmount, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Handle purchase receipt event and create corresponding accounting entries
    /// </summary>
    Task HandlePurchaseReceiptAsync(Guid purchaseOrderId, decimal totalAmount, decimal taxAmount, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Handle inventory adjustment event and create corresponding accounting entries
    /// </summary>
    Task HandleInventoryAdjustmentAsync(Guid itemId, decimal quantity, decimal unitCost, string adjustmentType, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Handle cash receipt event and create corresponding accounting entries
    /// </summary>
    Task HandleCashReceiptAsync(decimal amount, string reference, string description, Guid accountId, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Handle cash payment event and create corresponding accounting entries
    /// </summary>
    Task HandleCashPaymentAsync(decimal amount, string reference, string description, Guid accountId, string tenantId, CancellationToken cancellationToken = default);
}


