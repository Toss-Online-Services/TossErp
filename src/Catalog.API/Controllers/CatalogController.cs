using Catalog.Domain.DTOs;
using Catalog.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetItems(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? brand = null,
        [FromQuery] int? type = null)
    {
        var items = await _catalogService.GetCatalogItemsAsync(pageIndex, pageSize, brand, type);
        return Ok(items);
    }

    [HttpGet("items/{id:int}")]
    public async Task<ActionResult<CatalogItemDto>> GetItem(int id)
    {
        var item = await _catalogService.GetCatalogItemAsync(id);
        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpGet("items/by")]
    public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetItemsByIds([FromQuery] int[] ids)
    {
        var items = await _catalogService.GetCatalogItemsAsync(ids);
        return Ok(items);
    }

    [HttpGet("items/withsemanticrelevance")]
    public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetItemsWithSemanticRelevance(
        [FromQuery] string text,
        [FromQuery] int page = 0,
        [FromQuery] int take = 10)
    {
        var items = await _catalogService.GetCatalogItemsWithSemanticRelevanceAsync(page, take, text);
        return Ok(items);
    }

    [HttpGet("catalogbrands")]
    public async Task<ActionResult<IEnumerable<string>>> GetBrands()
    {
        var brands = await _catalogService.GetBrandsAsync();
        return Ok(brands);
    }

    [HttpGet("catalogtypes")]
    public async Task<ActionResult<IEnumerable<string>>> GetTypes()
    {
        var types = await _catalogService.GetTypesAsync();
        return Ok(types);
    }
} 
