using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;

namespace Toss.Application.ShoppingCart.Commands.UpdateCartItem;

public record UpdateCartItemCommand : IRequest<UpdateCartItemResult>
{
    public int CartItemId { get; init; }
    public int Quantity { get; init; }
    public string SessionId { get; init; } = string.Empty;
}

public record UpdateCartItemResult
{
    public bool Success { get; init; }
    public int TotalItems { get; init; }
    public decimal CartTotal { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, UpdateCartItemResult>
{
    private readonly IApplicationDbContext _context;

    public UpdateCartItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCartItemResult> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _context.ShoppingCartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == request.CartItemId && c.SessionId == request.SessionId && c.IsActive, cancellationToken);

        if (cartItem == null)
            throw new Common.Exceptions.NotFoundException(nameof(ShoppingCartItem), request.CartItemId);

        if (request.Quantity <= 0)
        {
            // Remove item
            _context.ShoppingCartItems.Remove(cartItem);
        }
        else
        {
            // Update quantity
            cartItem.Quantity = request.Quantity;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Calculate cart totals
        var cartItems = await _context.ShoppingCartItems
            .Where(c => c.SessionId == request.SessionId && c.IsActive)
            .ToListAsync(cancellationToken);

        var cartTotal = cartItems.Sum(c => c.GetTotal());
        var totalItems = cartItems.Sum(c => c.Quantity);

        return new UpdateCartItemResult
        {
            Success = true,
            TotalItems = totalItems,
            CartTotal = cartTotal,
            Message = request.Quantity <= 0 
                ? $"Removed {cartItem.Product?.Name} from cart"
                : $"Updated {cartItem.Product?.Name} quantity to {request.Quantity}"
        };
    }
}

