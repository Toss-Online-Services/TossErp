using Microsoft.AspNetCore.Mvc;
using TossErp.Application.DTOs;
using TossErp.Application.Services;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] Guid businessId)
    {
        try
        {
            var products = await _productService.GetByBusinessIdAsync(businessId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode, [FromQuery] Guid businessId)
    {
        try
        {
            var product = await _productService.GetByBarcodeAsync(barcode, businessId);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product by barcode {Barcode}", barcode);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts([FromQuery] Guid businessId)
    {
        try
        {
            var products = await _productService.GetLowStockProductsAsync(businessId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock products for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving low stock products");
        }
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category, [FromQuery] Guid businessId)
    {
        try
        {
            var products = await _productService.GetByCategoryAsync(category, businessId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by category {Category}", category);
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string term, [FromQuery] Guid businessId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var products = await _productService.SearchAsync(term, businessId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term {SearchTerm}", term);
            return StatusCode(500, "An error occurred while searching products");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto)
    {
        try
        {
            if (await _productService.BarcodeExistsAsync(createDto.Barcode, createDto.BusinessId))
                return BadRequest("A product with this barcode already exists");

            var product = await _productService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateDto)
    {
        try
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            var product = await _productService.UpdateAsync(id, updateDto);
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    [HttpPut("{id:guid}/stock")]
    public async Task<ActionResult> UpdateStock(Guid id, UpdateStockDto updateStockDto)
    {
        try
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.UpdateStockAsync(id, updateStockDto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", id);
            return StatusCode(500, "An error occurred while updating stock");
        }
    }

    [HttpPut("{id:guid}/minimum-stock")]
    public async Task<ActionResult> SetMinimumStockLevel(Guid id, [FromBody] int minimumLevel)
    {
        try
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.SetMinimumStockLevelAsync(id, minimumLevel);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting minimum stock level for product {ProductId}", id);
            return StatusCode(500, "An error occurred while setting minimum stock level");
        }
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<ActionResult> DeactivateProduct(Guid id)
    {
        try
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.DeactivateAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating product {ProductId}", id);
            return StatusCode(500, "An error occurred while deactivating the product");
        }
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<ActionResult> ActivateProduct(Guid id)
    {
        try
        {
            if (!await _productService.ExistsAsync(id))
                return NotFound();

            await _productService.ActivateAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating product {ProductId}", id);
            return StatusCode(500, "An error occurred while activating the product");
        }
    }
} 
