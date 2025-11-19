using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.ShoppingCart.Commands.Checkout;

public record CheckoutCommand : IRequest<CheckoutResult>
{
    public string SessionId { get; init; } = string.Empty;
    public int ShopId { get; init; }
    public int? CustomerId { get; init; }
    public string PaymentMethod { get; init; } = "Cash";
    public decimal AmountPaid { get; init; }
    public string? Notes { get; init; }
}

public record CheckoutResult
{
    public int SaleId { get; init; }
    public string SaleNumber { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal Change { get; init; }
    public DateTime CompletedAt { get; init; }
}

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly IApplicationDbContext _context;

    public CheckoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        // Get cart items
        var cartItems = await _context.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.SessionId == request.SessionId && c.ShopId == request.ShopId && c.IsActive)
            .ToListAsync(cancellationToken);

        if (!cartItems.Any())
            throw new BadRequestException("Cart is empty");

        // Calculate totals
        var total = cartItems.Sum(c => c.GetTotal());

        if (request.AmountPaid < total)
            throw new BadRequestException("Amount paid is less than total");

        // Create sale
        var sale = new Sale
        {
            ShopId = request.ShopId,
            CustomerId = request.CustomerId,
            SaleDate = DateTimeOffset.UtcNow,
            TotalAmount = total,
            PaymentMethod = Enum.TryParse<PaymentType>(request.PaymentMethod, out var paymentType) ? paymentType : PaymentType.Cash,
            Status = SaleStatus.Completed,
            Notes = request.Notes
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);

        // Create sale items
        foreach (var cartItem in cartItems)
        {
            var subtotal = cartItem.UnitPrice * cartItem.Quantity - cartItem.DiscountAmount;
            var taxAmount = subtotal * cartItem.TaxRate / 100;
            var lineTotal = subtotal + taxAmount;

            var saleItem = new SaleItem
            {
                SaleId = sale.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product?.Name ?? string.Empty,
                ProductSKU = cartItem.Product?.SKU,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                DiscountAmount = cartItem.DiscountAmount,
                TaxAmount = taxAmount,
                LineTotal = lineTotal
            };

            sale.Items.Add(saleItem);

            // Update stock level
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(
                    sl => sl.ShopId == request.ShopId && sl.ProductId == cartItem.ProductId,
                    cancellationToken);

            var quantityBefore = stockLevel?.Quantity ?? 0;
            var quantityChange = -cartItem.Quantity; // Negative for sales
            var quantityAfter = Math.Max(0, quantityBefore + quantityChange);

            if (stockLevel == null)
            {
                // Create new stock level if doesn't exist
                stockLevel = new StockLevel
                {
                    ShopId = request.ShopId,
                    ProductId = cartItem.ProductId,
                    Quantity = quantityAfter,
                    ReorderPoint = cartItem.Product?.MinimumStockLevel ?? 10,
                    ReorderQuantity = cartItem.Product?.ReorderQuantity ?? 20
                };
                _context.StockLevels.Add(stockLevel);
            }
            else
            {
                stockLevel.Quantity = quantityAfter;
            }

            // Record stock movement
            var stockMovement = new StockMovement
            {
                ProductId = cartItem.ProductId,
                ShopId = request.ShopId,
                QuantityBefore = quantityBefore,
                QuantityChange = quantityChange,
                QuantityAfter = quantityAfter,
                MovementType = StockMovementType.Sale,
                MovementDate = DateTimeOffset.UtcNow,
                ReferenceType = "Sale",
                ReferenceId = sale.Id,
                Notes = $"POS Sale checkout - Session: {request.SessionId}"     
            };

            _context.StockMovements.Add(stockMovement);

            // Mark cart item as checked out
            cartItem.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var saleNumber = $"S{sale.Id:D6}";
        var change = request.AmountPaid - total;

        return new CheckoutResult
        {
            SaleId = sale.Id,
            SaleNumber = saleNumber,
            Total = total,
            AmountPaid = request.AmountPaid,
            Change = change,
            CompletedAt = sale.SaleDate.DateTime
        };
    }
}

