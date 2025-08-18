using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.ValueObjects;

namespace TossErp.Procurement.Application.Commands.CreatePurchaseOrder;

/// <summary>
/// Handler for CreatePurchaseOrderCommand
/// </summary>
public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;
    private readonly ICurrentUserService _currentUserService;

    public CreatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService,
        ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
        _currentUserService = currentUserService;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate supplier exists
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null)
            throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found");

        // Validate supplier is active
        if (supplier.Status != Domain.Enums.SupplierStatus.Active)
            throw new InvalidOperationException($"Supplier {supplier.Name} is not active");

        // Get tenant ID from current user
        var tenantId = _currentUserService.TenantId ?? "default-tenant";
        var currentUser = _currentUserService.UserName ?? "system";

        // Generate purchase order number
        var purchaseOrderNumber = PurchaseOrderNumber.Generate(DateTime.UtcNow.Year, 1); // TODO: Get next sequence

        // Create purchase order
        var purchaseOrder = PurchaseOrder.Create(
            request.SupplierId,
            request.SupplierName,
            tenantId,
            request.PaymentTerms,
            purchaseOrderNumber.Value);

        // Add items to purchase order
        foreach (var itemRequest in request.Items)
        {
            purchaseOrder.AddItem(
                itemRequest.ItemId,
                itemRequest.ItemName,
                itemRequest.ItemSku,
                itemRequest.Quantity,
                itemRequest.UnitPrice,
                itemRequest.TaxRate,
                itemRequest.DiscountPercentage,
                itemRequest.ExpectedDeliveryDate);
        }

        // Set expected delivery date if provided
        if (request.ExpectedDeliveryDate.HasValue)
        {
            purchaseOrder.UpdateExpectedDeliveryDate(request.ExpectedDeliveryDate.Value, currentUser);
        }

        // Add notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            purchaseOrder.AddNotes(request.Notes, currentUser);
        }

        // Save purchase order
        await _purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        await _domainEventService.PublishAsync(purchaseOrder.DomainEvents, cancellationToken);
        purchaseOrder.ClearDomainEvents();

        // Return DTO
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
