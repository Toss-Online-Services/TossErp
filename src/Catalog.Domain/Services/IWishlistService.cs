using Catalog.Domain.DTOs;

namespace Catalog.Domain.Services;

public interface IWishlistService
{
    Task<IEnumerable<WishlistItemDto>> GetWishlistAsync(string userId);
    Task<WishlistItemDto> AddToWishlistAsync(string userId, int catalogItemId);
    Task RemoveFromWishlistAsync(string userId, int catalogItemId);
    Task<bool> IsInWishlistAsync(string userId, int catalogItemId);
} 
