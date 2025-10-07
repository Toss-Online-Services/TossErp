using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Inventory;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Products
            .Include(p => p.StockLevels)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                (p.Sku != null && p.Sku.Contains(search)) ||
                (p.Barcode != null && p.Barcode.Contains(search)));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(products);
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Get product by SKU or Barcode
    /// </summary>
    [HttpGet("search/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProductByCode(string code)
    {
        var product = await _context.Products
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.Sku == code || p.Barcode == code);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = new Product
            {
                Name = request.Name,
                Sku = request.Sku,
                Barcode = request.Barcode,
                Description = request.Description,
                Type = request.Type,
                CategoryId = request.CategoryId,
                CategoryName = request.CategoryName,
                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                WholesalePrice = request.WholesalePrice,
                TrackInventory = request.TrackInventory,
                ReorderPoint = request.ReorderPoint,
                ReorderQuantity = request.ReorderQuantity,
                Weight = request.Weight,
                WeightUnit = request.WeightUnit,
                Dimensions = request.Dimensions,
                IsTaxable = request.IsTaxable,
                TaxRate = request.TaxRate,
                TaxCategory = request.TaxCategory,
                IsActive = request.IsActive,
                ImageUrl = request.ImageUrl,
                AdditionalImages = request.AdditionalImages ?? new List<string>(),
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created product {ProductName} with SKU {Sku}", product.Name, product.Sku);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return BadRequest(new { error = "Failed to create product", message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        try
        {
            product.Name = request.Name;
            product.Sku = request.Sku;
            product.Barcode = request.Barcode;
            product.Description = request.Description;
            product.CategoryId = request.CategoryId;
            product.CategoryName = request.CategoryName;
            product.CostPrice = request.CostPrice;
            
            // Use the domain method for price changes to trigger events
            if (product.SellingPrice != request.SellingPrice)
            {
                product.UpdatePrice(request.SellingPrice, User.Identity?.Name ?? "System");
            }
            
            product.WholesalePrice = request.WholesalePrice;
            product.ReorderPoint = request.ReorderPoint;
            product.ReorderQuantity = request.ReorderQuantity;
            product.IsTaxable = request.IsTaxable;
            product.TaxRate = request.TaxRate;
            product.ImageUrl = request.ImageUrl;
            product.AdditionalImages = request.AdditionalImages ?? new List<string>();
            product.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated product {ProductName}", product.Name);

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return BadRequest(new { error = "Failed to update product", message = ex.Message });
        }
    }

    /// <summary>
    /// Delete (soft delete) a product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        product.Deactivate();
        product.DeletedBy = User.Identity?.Name ?? "System";
        _context.Products.Remove(product); // Soft delete via BaseEntity

        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted product {ProductName}", product.Name);

        return NoContent();
    }

    /// <summary>
    /// Get stock levels for a product across all warehouses
    /// </summary>
    [HttpGet("{id}/stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<StockLevel>>> GetProductStock(int id)
    {
        var product = await _context.Products
            .Include(p => p.StockLevels)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        return Ok(product.StockLevels);
    }
}

// Request DTOs
public record CreateProductRequest(
    string Name,
    string? Sku,
    string? Barcode,
    string? Description,
    ProductType Type,
    int? CategoryId,
    string? CategoryName,
    decimal CostPrice,
    decimal SellingPrice,
    decimal? WholesalePrice,
    bool TrackInventory,
    int? ReorderPoint,
    int? ReorderQuantity,
    decimal? Weight,
    string? WeightUnit,
    string? Dimensions,
    bool IsTaxable,
    decimal? TaxRate,
    string? TaxCategory,
    bool IsActive,
    string? ImageUrl,
    List<string>? AdditionalImages
);

public record UpdateProductRequest(
    string Name,
    string? Sku,
    string? Barcode,
    string? Description,
    int? CategoryId,
    string? CategoryName,
    decimal CostPrice,
    decimal SellingPrice,
    decimal? WholesalePrice,
    int? ReorderPoint,
    int? ReorderQuantity,
    bool IsTaxable,
    decimal? TaxRate,
    string? ImageUrl,
    List<string>? AdditionalImages
);

