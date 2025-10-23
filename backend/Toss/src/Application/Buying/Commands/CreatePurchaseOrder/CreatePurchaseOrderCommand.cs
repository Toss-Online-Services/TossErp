using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Buying;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Suppliers;
using Toss.Domain.Enums;

namespace Toss.Application.Buying.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand : IRequest<int>
{
    public int ShopId { get; init; }
    public int SupplierId { get; init; }
    public DateTimeOffset? ExpectedDeliveryDate { get; init; }
    public decimal ShippingCost { get; init; }
    public int? GroupBuyPoolId { get; init; }
    public string? Notes { get; init; }
    public List<PurchaseOrderItemDto> Items { get; init; } = new();
}

public record PurchaseOrderItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePurchaseOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate shop exists
        var shop = await _context.Shops.FindAsync(new object[] { request.ShopId }, cancellationToken);
        if (shop == null)
            throw new NotFoundException(nameof(Shop), request.ShopId.ToString());

        // Validate supplier exists
        var supplier = await _context.Suppliers.FindAsync(new object[] { request.SupplierId }, cancellationToken);
        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.SupplierId.ToString());

        var purchaseOrder = new PurchaseOrder
        {
            PONumber = await GeneratePONumber(request.ShopId, cancellationToken),
            ShopId = request.ShopId,
            SupplierId = request.SupplierId,
            OrderDate = DateTimeOffset.UtcNow,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Status = PurchaseOrderStatus.Draft,
            ShippingCost = request.ShippingCost,
            GroupBuyPoolId = request.GroupBuyPoolId,
            IsPartOfGroupBuy = request.GroupBuyPoolId.HasValue,
            Notes = request.Notes
        };

        decimal subtotal = 0;
        decimal taxTotal = 0;

        foreach (var itemDto in request.Items)
        {
            var product = await _context.Products
                .FindAsync(new object[] { itemDto.ProductId }, cancellationToken);

            if (product == null)
                throw new NotFoundException(nameof(Product), itemDto.ProductId.ToString());

            var lineTotal = itemDto.UnitPrice * itemDto.Quantity;
            var taxAmount = product.IsTaxable ? lineTotal * 0.15m : 0; // 15% VAT

            var item = new PurchaseOrderItem
            {
                ProductId = itemDto.ProductId,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                QuantityOrdered = itemDto.Quantity,
                QuantityReceived = 0,
                UnitPrice = itemDto.UnitPrice,
                TaxAmount = taxAmount,
                LineTotal = lineTotal + taxAmount
            };

            purchaseOrder.Items.Add(item);
            subtotal += lineTotal;
            taxTotal += taxAmount;
        }

        purchaseOrder.Subtotal = subtotal;
        purchaseOrder.TaxAmount = taxTotal;
        purchaseOrder.Total = subtotal + taxTotal + request.ShippingCost;

        _context.PurchaseOrders.Add(purchaseOrder);
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

