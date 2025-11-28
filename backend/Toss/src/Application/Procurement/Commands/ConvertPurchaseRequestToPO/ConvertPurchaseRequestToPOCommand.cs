using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Procurement.Commands.ConvertPurchaseRequestToPO;

public record ConvertPurchaseRequestToPOCommand : IRequest<int>
{
    public int PurchaseRequestId { get; init; }
    public DateTimeOffset? ExpectedDeliveryDate { get; init; }
    public decimal ShippingCost { get; init; }
    public string? Notes { get; init; }
}

public class ConvertPurchaseRequestToPOCommandHandler : IRequestHandler<ConvertPurchaseRequestToPOCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public ConvertPurchaseRequestToPOCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(ConvertPurchaseRequestToPOCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Load PurchaseRequest with related data
        var purchaseRequest = await _context.PurchaseRequests
            .Include(pr => pr.Shop)
            .Include(pr => pr.Vendor)
            .Include(pr => pr.Items)
                .ThenInclude(prl => prl.Item)
            .FirstOrDefaultAsync(pr => pr.Id == request.PurchaseRequestId, cancellationToken);

        if (purchaseRequest == null)
        {
            throw new NotFoundException(nameof(PurchaseRequest), request.PurchaseRequestId);
        }

        // Verify business context
        if (purchaseRequest.Shop.BusinessId != _businessContext.CurrentBusinessId)
        {
            throw new ForbiddenAccessException("Purchase request does not belong to the current business.");
        }

        // Check if already converted
        if (purchaseRequest.PurchaseOrderId.HasValue)
        {
            throw new ValidationException($"Purchase request {purchaseRequest.PRNumber} has already been converted to PO {purchaseRequest.PurchaseOrderId}.");
        }

        // Validate status - must be Approved or Submitted (we'll auto-approve Submitted)
        if (purchaseRequest.Status == PurchaseRequestStatus.Cancelled)
        {
            throw new ValidationException($"Cannot convert cancelled purchase request {purchaseRequest.PRNumber}.");
        }

        if (purchaseRequest.Status == PurchaseRequestStatus.ConvertedToPO)
        {
            throw new ValidationException($"Purchase request {purchaseRequest.PRNumber} has already been converted.");
        }

        // Auto-approve if in Submitted status
        if (purchaseRequest.Status == PurchaseRequestStatus.Submitted)
        {
            purchaseRequest.Status = PurchaseRequestStatus.Approved;
        }

        // Validate items exist
        if (purchaseRequest.Items == null || !purchaseRequest.Items.Any())
        {
            throw new ValidationException($"Purchase request {purchaseRequest.PRNumber} has no items.");
        }

        // Validate all products exist and belong to business
        foreach (var prLine in purchaseRequest.Items)
        {
            if (prLine.Item == null)
            {
                throw new NotFoundException($"Product {prLine.ItemId} not found for purchase request line {prLine.Id}.");
            }

            if (prLine.Item.BusinessId != _businessContext.CurrentBusinessId)
            {
                throw new ForbiddenAccessException($"Product {prLine.ItemId} does not belong to the current business.");
            }
        }

        // Generate PO number
        var poNumber = await GeneratePONumber(purchaseRequest.ShopId, cancellationToken);

        // Create PurchaseOrder
        var purchaseOrder = new PurchaseOrder
        {
            PONumber = poNumber,
            ShopId = purchaseRequest.ShopId,
            VendorId = purchaseRequest.VendorId,
            OrderDate = DateTimeOffset.UtcNow,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate ?? purchaseRequest.RequiredByDate,
            RequiredDate = purchaseRequest.RequiredByDate,
            Status = PurchaseOrderStatus.Draft,
            ShippingCost = request.ShippingCost,
            Notes = request.Notes ?? purchaseRequest.Notes,
            IsPartOfGroupBuy = false
        };

        decimal subtotal = 0;
        decimal taxTotal = 0;

        // Create PurchaseOrderItems from PurchaseRequestLines
        foreach (var prLine in purchaseRequest.Items)
        {
            var product = prLine.Item;
            
            // For MVP, we'll use the product's cost price if available, otherwise estimate from base price
            // In a real system, you'd fetch vendor pricing from VendorPricing table
            var unitPrice = product.CostPrice ?? (product.BasePrice * 0.7m); // Use cost price or estimate 70% of base price as cost
            
            var quantity = (int)Math.Ceiling(prLine.QuantityRequested); // Convert decimal to int (round up)
            var lineTotal = unitPrice * quantity;
            var taxAmount = product.IsTaxable ? lineTotal * 0.15m : 0; // 15% VAT

            var poItem = new PurchaseOrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                QuantityOrdered = quantity,
                QuantityReceived = 0,
                UnitPrice = unitPrice,
                TaxAmount = taxAmount,
                LineTotal = lineTotal + taxAmount
            };

            purchaseOrder.Items.Add(poItem);
            subtotal += lineTotal;
            taxTotal += taxAmount;
        }

        purchaseOrder.Subtotal = subtotal;
        purchaseOrder.TaxAmount = taxTotal;
        purchaseOrder.Total = subtotal + taxTotal + request.ShippingCost;

        _context.PurchaseOrders.Add(purchaseOrder);
        
        // Update PurchaseRequest to link to PO and mark as converted
        purchaseRequest.PurchaseOrderId = purchaseOrder.Id;
        purchaseRequest.Status = PurchaseRequestStatus.ConvertedToPO;

        await _context.SaveChangesAsync(cancellationToken);

        return purchaseOrder.Id;
    }

    private async Task<string> GeneratePONumber(int shopId, CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var count = await _context.PurchaseOrders
            .Where(po => po.ShopId == shopId && po.OrderDate.Date == date.Date)
            .CountAsync(cancellationToken);

        return $"PO-{shopId}-{date:yyyyMMdd}-{count + 1:D4}";
    }
}

