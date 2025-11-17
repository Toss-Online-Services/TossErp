using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.ShoppingCart.Commands.AddToCart;

public record AddToCartCommand : IRequest<AddToCartResult>
{
    public int ShopId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; } = 1;
    public string SessionId { get; init; } = string.Empty;
    public int? CustomerId { get; init; }
    public string? Attributes { get; init; }
}

public record AddToCartResult
{
    public int CartItemId { get; init; }
    public int TotalItems { get; init; }
    public decimal CartTotal { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, AddToCartResult>
{
    private readonly IApplicationDbContext _context;

    public AddToCartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AddToCartResult> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // Validate product exists
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
            throw new Common.Exceptions.NotFoundException(nameof(Product), request.ProductId);

        // Check if item already exists in cart
        var existingItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => 
                c.ProductId == request.ProductId &&
                c.SessionId == request.SessionId &&
                c.ShopId == request.ShopId &&
                c.IsActive,
                cancellationToken);

        ShoppingCartItem cartItem;

        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.Quantity;
            cartItem = existingItem;
        }
        else
        {
            // Create new cart item
            cartItem = new ShoppingCartItem
            {
                ShopId = request.ShopId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                SessionId = request.SessionId,
                CustomerId = request.CustomerId,
                UnitPrice = product.BasePrice,
                TaxRate = product.IsTaxable ? 15m : 0m, // South Africa VAT
                Attributes = request.Attributes,
                IsActive = true
            };

            _context.ShoppingCartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Calculate cart totals
        var cartItems = await _context.ShoppingCartItems
            .Where(c => c.SessionId == request.SessionId && c.IsActive)
            .ToListAsync(cancellationToken);

        var cartTotal = cartItems.Sum(c => c.GetTotal());
        var totalItems = cartItems.Sum(c => c.Quantity);

        return new AddToCartResult
        {
            CartItemId = cartItem.Id,
            TotalItems = totalItems,
            CartTotal = cartTotal,
            Message = existingItem != null 
                ? $"Updated {product.Name} quantity to {cartItem.Quantity}"
                : $"Added {product.Name} to cart"
        };
    }
}

