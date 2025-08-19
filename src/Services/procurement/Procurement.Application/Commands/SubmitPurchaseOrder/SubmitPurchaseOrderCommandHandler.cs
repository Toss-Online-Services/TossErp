using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;

namespace TossErp.Procurement.Application.Commands.SubmitPurchaseOrder;

/// <summary>
/// Handler for SubmitPurchaseOrderCommand
/// </summary>
public class SubmitPurchaseOrderCommandHandler : IRequestHandler<SubmitPurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;

    public SubmitPurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
    }

    public async Task<PurchaseOrderDto> Handle(SubmitPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        // Get purchase order
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder == null)
            throw new InvalidOperationException($"Purchase order with ID {request.PurchaseOrderId} not found");

        var currentUser = _currentUserService.UserName ?? "system";

        // Submit the purchase order
        purchaseOrder.Submit(currentUser);

        // Add notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            purchaseOrder.AddNotes(request.Notes, currentUser);
        }

        // Save changes
        await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(purchaseOrder.DomainEvents, cancellationToken);
        purchaseOrder.ClearDomainEvents();

        // Return updated DTO
        return MapToDto(purchaseOrder);
    }

    private static PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
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

    private static PurchaseOrderItemDto MapItemToDto(PurchaseOrderItem item)
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
