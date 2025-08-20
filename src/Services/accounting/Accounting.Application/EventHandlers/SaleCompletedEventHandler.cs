using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.EventHandlers;

/// <summary>
/// Event handler for SaleCompleted events from the Sales service
/// </summary>
public class SaleCompletedEventHandler : INotificationHandler<SaleCompletedEvent>
{
    private readonly IPostingRulesService _postingRulesService;
    private readonly ILogger<SaleCompletedEventHandler> _logger;

    public SaleCompletedEventHandler(
        IPostingRulesService postingRulesService,
        ILogger<SaleCompletedEventHandler> logger)
    {
        _postingRulesService = postingRulesService;
        _logger = logger;
    }

    public async Task Handle(SaleCompletedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received SaleCompleted event for sale {SaleId}", notification.SaleId);

            // Extract the total amount and tax amount from the sale
            // In a real implementation, you might need to call the Sales service to get these details
            // For now, we'll use placeholder values or extract from the event if available
            var totalAmount = notification.TotalAmount; // Assuming the event contains this
            var taxAmount = notification.TaxAmount; // Assuming the event contains this
            var tenantId = notification.TenantId;

            await _postingRulesService.HandleSaleCompletedAsync(
                notification.SaleId,
                totalAmount,
                taxAmount,
                tenantId,
                cancellationToken);

            _logger.LogInformation("Successfully processed SaleCompleted event for sale {SaleId}", notification.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SaleCompleted event for sale {SaleId}", notification.SaleId);
            throw;
        }
    }
}

/// <summary>
/// Placeholder event class - in a real implementation, this would come from the Sales service
/// </summary>
public class SaleCompletedEvent : INotification
{
    public Guid SaleId { get; }
    public decimal TotalAmount { get; }
    public decimal TaxAmount { get; }
    public string TenantId { get; }
    public DateTime OccurredOn { get; }

    public SaleCompletedEvent(Guid saleId, decimal totalAmount, decimal taxAmount, string tenantId)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
        TaxAmount = taxAmount;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}


