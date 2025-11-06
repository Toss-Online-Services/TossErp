using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Exceptions;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.CustomerOrders.Commands.CreateCustomerOrder;

public record CreateCustomerOrderCommand : IRequest<int>
{
    public int CustomerId { get; init; }
    public int ShopId { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public string? ShippingMethod { get; init; }
    public int? ShippingAddressId { get; init; }
    public int? BillingAddressId { get; init; }
    public string? Notes { get; init; }
    public string PaymentMethod { get; init; } = "Cash";
}

public record OrderItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public class CreateCustomerOrderCommandHandler : IRequestHandler<CreateCustomerOrderCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCustomerOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCustomerOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate customer and shop exist
        var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellationToken);
        if (customer == null)
            throw new Common.Exceptions.NotFoundException(nameof(Customer), request.CustomerId.ToString());

        var shop = await _context.Stores.FindAsync(new object[] { request.ShopId }, cancellationToken);
        if (shop == null)
            throw new Common.Exceptions.NotFoundException(nameof(Store), request.ShopId.ToString());

        // Calculate totals
        decimal subtotalExclTax = 0;
        decimal taxAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
            if (product == null)
                throw new Common.Exceptions.NotFoundException(nameof(Product), item.ProductId.ToString());

            decimal itemSubtotal = item.UnitPrice * item.Quantity;
            subtotalExclTax += itemSubtotal;
            
            if (product.IsTaxable)
            {
                taxAmount += itemSubtotal * 0.15m; // 15% VAT
            }
        }

        decimal subtotalInclTax = subtotalExclTax + taxAmount;

        var order = new Order
        {
            OrderGuid = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            BillingAddressId = request.BillingAddressId,
            ShippingAddressId = request.ShippingAddressId,
            OrderStatus = OrderStatus.Pending,
            ShippingStatus = request.ShippingAddressId.HasValue ? ShippingStatus.NotYetShipped : ShippingStatus.ShippingNotRequired,
            PaymentStatus = Domain.Enums.PaymentStatus.Pending,
            PaymentMethodSystemName = request.PaymentMethod,
            OrderSubtotalExclTax = subtotalExclTax,
            OrderSubtotalInclTax = subtotalInclTax,
            OrderTax = taxAmount,
            OrderTotal = subtotalInclTax,
            ShippingMethod = request.ShippingMethod,
            CustomerCurrencyCode = "ZAR",
            Deleted = false
        };

        // Add order items
        foreach (var itemDto in request.Items)
        {
            var product = await _context.Products.FindAsync(new object[] { itemDto.ProductId }, cancellationToken);
            var taxAmount_item = product!.IsTaxable ? (itemDto.UnitPrice * itemDto.Quantity * 0.15m) : 0;

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPriceExclTax = itemDto.UnitPrice,
                UnitPriceInclTax = product.IsTaxable ? itemDto.UnitPrice * 1.15m : itemDto.UnitPrice,
                PriceExclTax = itemDto.UnitPrice * itemDto.Quantity,
                PriceInclTax = (itemDto.UnitPrice * itemDto.Quantity) + taxAmount_item,
                DiscountAmountExclTax = 0,
                DiscountAmountInclTax = 0
            };

            order.OrderItems.Add(orderItem);
        }

        // Add order notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            order.OrderNotes.Add(new OrderNote
            {
                Note = request.Notes,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

