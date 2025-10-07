using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Sales;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c =>
                c.Name.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.Phone != null && c.Phone.Contains(search)) ||
                (c.CompanyName != null && c.CompanyName.Contains(search)));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var customers = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(customers);
    }

    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Sales.Take(10).OrderByDescending(s => s.SaleDate))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = new Customer
            {
                Name = request.Name,
                Type = request.Type,
                Email = request.Email,
                Phone = request.Phone,
                AlternatePhone = request.AlternatePhone,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Country = request.Country ?? "South Africa",
                CompanyName = request.CompanyName,
                TaxNumber = request.TaxNumber,
                RegistrationNumber = request.RegistrationNumber,
                CreditLimit = request.CreditLimit,
                PaymentTermsDays = request.PaymentTermsDays,
                PreferredPaymentMethod = request.PreferredPaymentMethod,
                PreferredLanguage = request.PreferredLanguage ?? "English",
                ReceiveMarketing = request.ReceiveMarketing,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created customer {CustomerName}", customer.Name);

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return BadRequest(new { error = "Failed to create customer", message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        try
        {
            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.AlternatePhone = request.AlternatePhone;
            customer.AddressLine1 = request.AddressLine1;
            customer.AddressLine2 = request.AddressLine2;
            customer.City = request.City;
            customer.Province = request.Province;
            customer.PostalCode = request.PostalCode;
            customer.CreditLimit = request.CreditLimit;
            customer.PaymentTermsDays = request.PaymentTermsDays;
            customer.PreferredPaymentMethod = request.PreferredPaymentMethod;
            customer.PreferredLanguage = request.PreferredLanguage;
            customer.ReceiveMarketing = request.ReceiveMarketing;
            customer.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated customer {CustomerName}", customer.Name);

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return BadRequest(new { error = "Failed to update customer", message = ex.Message });
        }
    }

    /// <summary>
    /// Add loyalty points to customer
    /// </summary>
    [HttpPost("{id}/loyalty/add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLoyaltyPoints(int id, [FromBody] LoyaltyPointsRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        try
        {
            customer.AddLoyaltyPoints(request.Points);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added {Points} loyalty points to customer {CustomerName}", request.Points, customer.Name);

            return Ok(new { loyaltyPoints = customer.LoyaltyPoints });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Redeem loyalty points from customer
    /// </summary>
    [HttpPost("{id}/loyalty/redeem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RedeemLoyaltyPoints(int id, [FromBody] LoyaltyPointsRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        try
        {
            customer.RedeemLoyaltyPoints(request.Points);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Redeemed {Points} loyalty points from customer {CustomerName}", request.Points, customer.Name);

            return Ok(new { loyaltyPoints = customer.LoyaltyPoints });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

// Request DTOs
public record CreateCustomerRequest(
    string Name,
    CustomerType Type,
    string? Email,
    string? Phone,
    string? AlternatePhone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    string? Country,
    string? CompanyName,
    string? TaxNumber,
    string? RegistrationNumber,
    decimal CreditLimit,
    int PaymentTermsDays,
    string? PreferredPaymentMethod,
    string? PreferredLanguage,
    bool ReceiveMarketing
);

public record UpdateCustomerRequest(
    string Name,
    string? Email,
    string? Phone,
    string? AlternatePhone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    decimal CreditLimit,
    int PaymentTermsDays,
    string? PreferredPaymentMethod,
    string? PreferredLanguage,
    bool ReceiveMarketing
);

public record LoyaltyPointsRequest(int Points);

