using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;

namespace TossErp.Procurement.Application.Queries.GetPurchaseOrderById;

/// <summary>
/// Handler for GetPurchaseOrderByIdQuery
/// </summary>
public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository purchaseOrderRepository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (purchaseOrder == null)
            return null;

        return MapToDto(purchaseOrder);
    }

    private static PurchaseOrderDto MapToDto(Domain.Entities.PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber.Value,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName = purchaseOrder.SupplierName,
            Status = purchaseOrder.Status,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            ActualDeliveryDate = purchaseOrder.ActualDeliveryDate,
            PaymentTerms = purchaseOrder.PaymentTerms,
            Notes = purchaseOrder.Notes,
            ApprovalNotes = purchaseOrder.ApprovalNotes,
            ApprovedAt = purchaseOrder.ApprovedAt,
            ApprovedBy = purchaseOrder.ApprovedBy,
            SentAt = purchaseOrder.SentAt,
            SentBy = purchaseOrder.SentBy,
            CancelledAt = purchaseOrder.CancelledAt,
            CancellationReason = purchaseOrder.CancellationReason,
            CancelledBy = purchaseOrder.CancelledBy,
            Subtotal = purchaseOrder.Subtotal,
            TotalDiscount = purchaseOrder.TotalDiscount,
            SubtotalAfterDiscount = purchaseOrder.SubtotalAfterDiscount,
            TotalTax = purchaseOrder.TotalTax,
            TotalAmount = purchaseOrder.TotalAmount,
            TotalReceivedQuantity = purchaseOrder.TotalReceivedQuantity,
            TotalRemainingQuantity = purchaseOrder.TotalRemainingQuantity,
            IsFullyReceived = purchaseOrder.IsFullyReceived,
            IsPartiallyReceived = purchaseOrder.IsPartiallyReceived,
            Items = purchaseOrder.Items.Select(MapItemToDto).ToList(),
            CreatedAt = purchaseOrder.CreatedAt,
            UpdatedAt = purchaseOrder.UpdatedAt,
            CreatedBy = purchaseOrder.CreatedBy,
            UpdatedBy = purchaseOrder.UpdatedBy,
            TenantId = purchaseOrder.TenantId
        };
    }

    private static PurchaseOrderItemDto MapItemToDto(Domain.Entities.PurchaseOrderItem item)
    {
        return new PurchaseOrderItemDto
        {
            Id = item.Id,
            PurchaseOrderId = item.PurchaseOrderId,
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemSku = item.ItemSku,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TaxRate = item.TaxRate,
            ReceivedQuantity = item.ReceivedQuantity,
            DiscountPercentage = item.DiscountPercentage,
            Notes = item.Notes,
            ExpectedDeliveryDate = item.ExpectedDeliveryDate,
            LineTotal = item.LineTotal,
            DiscountAmount = item.DiscountAmount,
            SubtotalAfterDiscount = item.SubtotalAfterDiscount,
            TaxAmount = item.TaxAmount,
            TotalAmount = item.TotalAmount,
            IsFullyReceived = item.IsFullyReceived,
            RemainingQuantity = item.RemainingQuantity
        };
    }
}
