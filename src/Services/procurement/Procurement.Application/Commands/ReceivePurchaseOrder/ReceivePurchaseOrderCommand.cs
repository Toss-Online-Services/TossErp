using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Commands.ReceivePurchaseOrder;

/// <summary>
/// Command to receive items from a purchase order
/// </summary>
public class ReceivePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public string? ReceiptNumber { get; set; }
    public string? Notes { get; set; }
    public List<ReceivePurchaseOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request for receiving a specific item from a purchase order
/// </summary>
public class ReceivePurchaseOrderItemRequest
{
    public Guid PurchaseOrderItemId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal? UnitPrice { get; set; } // Optional override of original unit price
    public string? Notes { get; set; }
}
