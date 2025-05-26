using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface ICartService
{
    Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string userId);
    Task<CartItemDto> AddToCartAsync(string userId, int catalogItemId, int quantity);
    Task<CartItemDto> UpdateCartItemQuantityAsync(string userId, int cartItemId, int quantity);
    Task RemoveFromCartAsync(string userId, int cartItemId);
    Task ClearCartAsync(string userId);
    Task<decimal> GetCartTotalAsync(string userId);
    Task<int> GetCartItemCountAsync(string userId);
} 
