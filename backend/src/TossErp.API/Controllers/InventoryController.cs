using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Inventory;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(ApplicationDbContext context, ILogger<InventoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Stock Levels

    /// <summary>
    /// Get stock levels with optional filtering
    /// </summary>
    [HttpGet("stock-levels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetStockLevels(
        [FromQuery] int? warehouseId = null,
        [FromQuery] bool? lowStockOnly = null)
    {
        var query = _context.StockLevels
            .Include(s => s.Product)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId.Value);

        var stockLevels = await query.ToListAsync();

        if (lowStockOnly == true)
            stockLevels = stockLevels.Where(s => s.IsLowStock()).ToList();

        var result = stockLevels.Select(s => new
        {
            s.Id,
            s.ProductId,
            productName = s.Product.Name,
            productSku = s.Product.Sku,
            s.WarehouseId,
            s.WarehouseName,
            s.QuantityOnHand,
            s.QuantityReserved,
            quantityAvailable = s.QuantityAvailable,
            s.ReorderPoint,
            s.ReorderQuantity,
            isLowStock = s.IsLowStock(),
            s.LastCountDate
        });

        return Ok(result);
    }

    /// <summary>
    /// Get stock level for a specific product and warehouse
    /// </summary>
    [HttpGet("stock-levels/product/{productId}/warehouse/{warehouseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockLevel>> GetStockLevel(int productId, int warehouseId)
    {
        var stockLevel = await _context.StockLevels
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);

        if (stockLevel == null)
            return NotFound();

        return Ok(new
        {
            stockLevel.Id,
            stockLevel.ProductId,
            productName = stockLevel.Product.Name,
            stockLevel.WarehouseId,
            stockLevel.WarehouseName,
            stockLevel.QuantityOnHand,
            stockLevel.QuantityReserved,
            quantityAvailable = stockLevel.QuantityAvailable,
            stockLevel.ReorderPoint,
            isLowStock = stockLevel.IsLowStock()
        });
    }

    /// <summary>
    /// Adjust stock level
    /// </summary>
    [HttpPost("stock-levels/{id}/adjust")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockRequest request)
    {
        var stockLevel = await _context.StockLevels.FindAsync(id);

        if (stockLevel == null)
            return NotFound();

        try
        {
            stockLevel.AdjustStock(request.Quantity, request.Reason, User.Identity?.Name ?? "System");
            
            // Create stock movement record
            var movement = new StockMovement
            {
                ProductId = stockLevel.ProductId,
                ToWarehouseId = stockLevel.WarehouseId,
                ToWarehouseName = stockLevel.WarehouseName,
                Quantity = request.Quantity,
                MovementType = "Adjustment",
                Notes = request.Reason,
                MovementDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            };
            
            _context.StockMovements.Add(movement);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Adjusted stock for product {ProductId} at warehouse {WarehouseId} by {Quantity}", 
                stockLevel.ProductId, stockLevel.WarehouseId, request.Quantity);

            return Ok(stockLevel);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Stock Movements

    /// <summary>
    /// Get stock movements
    /// </summary>
    [HttpGet("stock-movements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockMovement>>> GetStockMovements(
        [FromQuery] int? productId = null,
        [FromQuery] int? warehouseId = null,
        [FromQuery] string? movementType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.StockMovements
            .Include(m => m.Product)
            .AsQueryable();

        if (productId.HasValue)
            query = query.Where(m => m.ProductId == productId.Value);

        if (warehouseId.HasValue)
            query = query.Where(m => m.ToWarehouseId == warehouseId.Value || m.FromWarehouseId == warehouseId.Value);

        if (!string.IsNullOrEmpty(movementType))
            query = query.Where(m => m.MovementType == movementType);

        if (startDate.HasValue)
            query = query.Where(m => m.MovementDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(m => m.MovementDate <= endDate.Value);

        var movements = await query
            .OrderByDescending(m => m.MovementDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(movements);
    }

    /// <summary>
    /// Create a stock transfer between warehouses
    /// </summary>
    [HttpPost("stock-movements/transfer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StockMovement>> CreateStockTransfer([FromBody] StockTransferRequest request)
    {
        try
        {
            // Get source stock level
            var sourceStock = await _context.StockLevels
                .FirstOrDefaultAsync(s => s.ProductId == request.ProductId && s.WarehouseId == request.FromWarehouseId);

            if (sourceStock == null)
                return BadRequest(new { error = "Source stock level not found" });

            if (sourceStock.QuantityAvailable < request.Quantity)
                return BadRequest(new { error = $"Insufficient stock. Available: {sourceStock.QuantityAvailable}" });

            // Get or create destination stock level
            var destStock = await _context.StockLevels
                .FirstOrDefaultAsync(s => s.ProductId == request.ProductId && s.WarehouseId == request.ToWarehouseId);

            if (destStock == null)
            {
                var destWarehouse = await _context.Warehouses.FindAsync(request.ToWarehouseId);
                if (destWarehouse == null)
                    return BadRequest(new { error = "Destination warehouse not found" });

                destStock = new StockLevel
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.ToWarehouseId,
                    WarehouseName = destWarehouse.Name,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    CreatedBy = User.Identity?.Name ?? "System"
                };
                _context.StockLevels.Add(destStock);
            }

            // Adjust stock levels
            sourceStock.AdjustStock(-request.Quantity, $"Transfer to {destStock.WarehouseName}", User.Identity?.Name ?? "System");
            destStock.AdjustStock(request.Quantity, $"Transfer from {sourceStock.WarehouseName}", User.Identity?.Name ?? "System");

            // Create movement record
            var sourceWarehouse = await _context.Warehouses.FindAsync(request.FromWarehouseId);
            var movement = new StockMovement
            {
                ProductId = request.ProductId,
                FromWarehouseId = request.FromWarehouseId,
                FromWarehouseName = sourceWarehouse?.Name,
                ToWarehouseId = request.ToWarehouseId,
                ToWarehouseName = destStock.WarehouseName,
                Quantity = request.Quantity,
                MovementType = "Transfer",
                Notes = request.Notes,
                MovementDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.StockMovements.Add(movement);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Transferred {Quantity} units of product {ProductId} from warehouse {From} to {To}", 
                request.Quantity, request.ProductId, request.FromWarehouseId, request.ToWarehouseId);

            return CreatedAtAction(nameof(GetStockMovements), movement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating stock transfer");
            return BadRequest(new { error = "Failed to create stock transfer", message = ex.Message });
        }
    }

    #endregion

    #region Warehouses

    /// <summary>
    /// Get all warehouses
    /// </summary>
    [HttpGet("warehouses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses([FromQuery] bool? isActive = null)
    {
        var query = _context.Warehouses.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(w => w.IsActive == isActive.Value);

        var warehouses = await query
            .OrderBy(w => w.Name)
            .ToListAsync();

        return Ok(warehouses);
    }

    /// <summary>
    /// Create a new warehouse
    /// </summary>
    [HttpPost("warehouses")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Warehouse>> CreateWarehouse([FromBody] CreateWarehouseRequest request)
    {
        try
        {
            var warehouse = new Warehouse
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Email = request.Email,
                WarehouseType = request.WarehouseType,
                IsActive = true,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created warehouse {WarehouseName}", warehouse.Name);

            return CreatedAtAction(nameof(GetWarehouses), warehouse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating warehouse");
            return BadRequest(new { error = "Failed to create warehouse", message = ex.Message });
        }
    }

    #endregion
}

// Request DTOs
public record AdjustStockRequest(int Quantity, string Reason);

public record StockTransferRequest(
    int ProductId,
    int FromWarehouseId,
    int ToWarehouseId,
    int Quantity,
    string? Notes
);

public record CreateWarehouseRequest(
    string Name,
    string? Code,
    string? Description,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    string? ContactPerson,
    string? Phone,
    string? Email,
    string? WarehouseType
);

