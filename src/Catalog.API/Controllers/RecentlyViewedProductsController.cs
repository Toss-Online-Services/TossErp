using Catalog.API.Services;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecentlyViewedProductsController : ControllerBase
{
    private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;

    public RecentlyViewedProductsController(IRecentlyViewedProductsService recentlyViewedProductsService)
    {
        _recentlyViewedProductsService = recentlyViewedProductsService;
    }

    [HttpGet("{customerId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetRecentlyViewedProducts(
        int customerId,
        [FromQuery] int number = 20,
        [FromQuery] bool showHidden = false,
        [FromQuery] int? storeId = null,
        [FromQuery] int? languageId = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var products = await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(
            customerId, number, showHidden, storeId, languageId, pageIndex, pageSize);
        return Ok(products);
    }

    [HttpPost("{customerId}/products/{productId}")]
    public async Task<IActionResult> AddProductToRecentlyViewed(int customerId, int productId)
    {
        await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(customerId, productId);
        return NoContent();
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> ClearRecentlyViewedProducts(int customerId)
    {
        await _recentlyViewedProductsService.ClearRecentlyViewedProductsAsync(customerId);
        return NoContent();
    }
} 
