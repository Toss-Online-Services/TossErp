using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.EventHandlers;

/// <summary>
/// Event handler for InventoryAdjustment events
/// </summary>
public class InventoryAdjustmentEventHandler : INotificationHandler<InventoryAdjustmentEvent>
{
    private readonly IPostingRulesService _postingRulesService;
    private readonly ILogger<InventoryAdjustmentEventHandler> _logger;

    public InventoryAdjustmentEventHandler(
        IPostingRulesService postingRulesService,
        ILogger<InventoryAdjustmentEventHandler> logger)
    {
        _postingRulesService = postingRulesService;
        _logger = logger;
    }

    public async Task Handle(InventoryAdjustmentEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Received InventoryAdjustment event for item {ItemId}", notification.ItemId);

            await _postingRulesService.HandleInventoryAdjustmentAsync(
                notification.ItemId,
                notification.Quantity,
                notification.UnitCost,
                notification.AdjustmentType,
                notification.TenantId,
                cancellationToken);

            _logger.LogInformation("Successfully processed InventoryAdjustment event for item {ItemId}", notification.ItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing InventoryAdjustment event for item {ItemId}", notification.ItemId);
            throw;
        }
    }
}

/// <summary>
/// Placeholder event class - in a real implementation, this would come from the Inventory service
/// </summary>
public class InventoryAdjustmentEvent : INotification
{
    public Guid ItemId { get; }
    public decimal Quantity { get; }
    public decimal UnitCost { get; }
    public string AdjustmentType { get; }
    public string TenantId { get; }
    public DateTime OccurredOn { get; }

    public InventoryAdjustmentEvent(Guid itemId, decimal quantity, decimal unitCost, string adjustmentType, string tenantId)
    {
        ItemId = itemId;
        Quantity = quantity;
        UnitCost = unitCost;
        AdjustmentType = adjustmentType;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
    }
}


