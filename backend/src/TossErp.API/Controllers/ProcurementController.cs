using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Procurement;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProcurementController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProcurementController> _logger;

    public ProcurementController(ApplicationDbContext context, ILogger<ProcurementController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Suppliers

    /// <summary>
    /// Get all suppliers
    /// </summary>
    [HttpGet("suppliers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers(
        [FromQuery] SupplierStatus? status = null,
        [FromQuery] string? search = null)
    {
        var query = _context.Suppliers.AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s =>
                s.Name.Contains(search) ||
                (s.Email != null && s.Email.Contains(search)) ||
                (s.Phone != null && s.Phone.Contains(search)));
        }

        var suppliers = await query
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(suppliers);
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost("suppliers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Supplier>> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                Type = request.Type,
                Email = request.Email,
                Phone = request.Phone,
                Website = request.Website,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                TaxNumber = request.TaxNumber,
                RegistrationNumber = request.RegistrationNumber,
                PaymentTermsDays = request.PaymentTermsDays,
                ContactPersonName = request.ContactPersonName,
                ContactPersonPhone = request.ContactPersonPhone,
                ContactPersonEmail = request.ContactPersonEmail,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created supplier {SupplierName}", supplier.Name);

            return CreatedAtAction(nameof(GetSuppliers), supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier");
            return BadRequest(new { error = "Failed to create supplier", message = ex.Message });
        }
    }

    #endregion

    #region Purchase Orders

    /// <summary>
    /// Get all purchase orders
    /// </summary>
    [HttpGet("purchase-orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders(
        [FromQuery] PurchaseOrderStatus? status = null,
        [FromQuery] int? supplierId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (supplierId.HasValue)
            query = query.Where(p => p.SupplierId == supplierId.Value);

        var orders = await query
            .OrderByDescending(p => p.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(orders);
    }

    /// <summary>
    /// Create a new purchase order
    /// </summary>
    [HttpPost("purchase-orders")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        try
        {
            var order = new PurchaseOrder
            {
                OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}",
                SupplierId = request.SupplierId,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = request.ExpectedDeliveryDate,
                WarehouseId = request.WarehouseId,
                WarehouseName = request.WarehouseName,
                ShippingCost = request.ShippingCost,
                Terms = request.Terms,
                Notes = request.Notes,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            foreach (var itemRequest in request.Items)
            {
                order.Items.Add(new PurchaseOrderItem
                {
                    ProductId = itemRequest.ProductId,
                    ProductName = itemRequest.ProductName,
                    ProductSku = itemRequest.ProductSku,
                    QuantityOrdered = itemRequest.Quantity,
                    UnitPrice = itemRequest.UnitPrice,
                    LineTotal = itemRequest.Quantity * itemRequest.UnitPrice,
                    CreatedBy = User.Identity?.Name ?? "System"
                });
            }

            order.CalculateTotals(request.TaxRate);

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created purchase order {OrderNumber} for supplier {SupplierId}", order.OrderNumber, order.SupplierId);

            return CreatedAtAction(nameof(GetPurchaseOrders), order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            return BadRequest(new { error = "Failed to create purchase order", message = ex.Message });
        }
    }

    /// <summary>
    /// Approve a purchase order
    /// </summary>
    [HttpPost("purchase-orders/{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApprovePurchaseOrder(int id)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);

        if (order == null)
            return NotFound();

        try
        {
            order.Approve(User.Identity?.Name ?? "System");
            await _context.SaveChangesAsync();

            _logger.LogInformation("Approved purchase order {OrderNumber}", order.OrderNumber);

            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion
}

// Request DTOs
public record CreateSupplierRequest(
    string Name,
    SupplierType Type,
    string? Email,
    string? Phone,
    string? Website,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    string? TaxNumber,
    string? RegistrationNumber,
    int PaymentTermsDays,
    string? ContactPersonName,
    string? ContactPersonPhone,
    string? ContactPersonEmail
);

public record CreatePurchaseOrderRequest(
    int SupplierId,
    DateTime? ExpectedDeliveryDate,
    List<CreatePurchaseOrderItemRequest> Items,
    decimal ShippingCost,
    decimal TaxRate,
    int? WarehouseId,
    string? WarehouseName,
    string? Terms,
    string? Notes
);

public record CreatePurchaseOrderItemRequest(
    int ProductId,
    string ProductName,
    string? ProductSku,
    int Quantity,
    decimal UnitPrice
);

