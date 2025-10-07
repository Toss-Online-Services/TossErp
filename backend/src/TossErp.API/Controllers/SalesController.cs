using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Sales;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ApplicationDbContext context, ILogger<SalesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all sales with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales(
        [FromQuery] SaleStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(sales);
    }

    /// <summary>
    /// Get a specific sale by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return NotFound();

        return Ok(sale);
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Sale>> CreateSale([FromBody] CreateSaleRequest request)
    {
        try
        {
            var sale = new Sale
            {
                SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
                Status = SaleStatus.Draft,
                Type = request.Type,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                WarehouseId = request.WarehouseId,
                WarehouseName = request.WarehouseName,
                CashierId = request.CashierId,
                CashierName = request.CashierName,
                PosDeviceId = request.PosDeviceId,
                PosDeviceName = request.PosDeviceName,
                SaleDate = DateTime.UtcNow,
                Notes = request.Notes,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            foreach (var itemRequest in request.Items)
            {
                var item = new SaleItem
                {
                    ProductId = itemRequest.ProductId,
                    ProductName = itemRequest.ProductName,
                    ProductSku = itemRequest.ProductSku,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    Discount = itemRequest.Discount,
                    Notes = itemRequest.Notes
                };
                item.CalculateLineTotal();
                sale.Items.Add(item);
            }

            sale.DiscountAmount = request.DiscountAmount;
            sale.CalculateTotals(request.TaxRate);

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created sale {SaleNumber} for customer {CustomerName}", sale.SaleNumber, sale.CustomerName);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return BadRequest(new { error = "Failed to create sale", message = ex.Message });
        }
    }

    /// <summary>
    /// Complete a sale
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteSale(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return NotFound();

        try
        {
            sale.Complete();
            sale.UpdatedBy = User.Identity?.Name ?? "System";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Completed sale {SaleNumber}", sale.SaleNumber);

            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a sale
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSale(int id, [FromBody] CancelSaleRequest request)
    {
        var sale = await _context.Sales.FindAsync(id);

        if (sale == null)
            return NotFound();

        try
        {
            sale.Cancel(request.Reason);
            sale.UpdatedBy = User.Identity?.Name ?? "System";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cancelled sale {SaleNumber}: {Reason}", sale.SaleNumber, request.Reason);

            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get sales summary/analytics
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetSalesSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today;
        var end = endDate ?? DateTime.Today.AddDays(1);

        var sales = await _context.Sales
            .Where(s => s.SaleDate >= start && s.SaleDate < end)
            .Where(s => s.Status == SaleStatus.Completed)
            .ToListAsync();

        var summary = new
        {
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            AverageOrderValue = sales.Any() ? sales.Average(s => s.TotalAmount) : 0,
            TotalItemsSold = await _context.SaleItems
                .Where(i => i.Sale.SaleDate >= start && i.Sale.SaleDate < end)
                .SumAsync(i => i.Quantity),
            TopProducts = await _context.SaleItems
                .Where(i => i.Sale.SaleDate >= start && i.Sale.SaleDate < end)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    QuantitySold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.LineTotal)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync()
        };

        return Ok(summary);
    }
}

// Request DTOs
public record CreateSaleRequest(
    SaleType Type,
    int? CustomerId,
    string? CustomerName,
    string? CustomerPhone,
    string? CustomerEmail,
    List<CreateSaleItemRequest> Items,
    decimal DiscountAmount,
    decimal TaxRate,
    int? WarehouseId,
    string? WarehouseName,
    int? CashierId,
    string? CashierName,
    string? PosDeviceId,
    string? PosDeviceName,
    string? Notes
);

public record CreateSaleItemRequest(
    int? ProductId,
    string ProductName,
    string? ProductSku,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    string? Notes
);

public record CancelSaleRequest(string Reason);

