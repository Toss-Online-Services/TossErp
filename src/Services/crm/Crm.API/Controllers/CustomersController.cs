using Microsoft.AspNetCore.Mvc;
using Crm.Application.DTOs;

namespace Crm.API.Controllers;

/// <summary>
/// API controller for customer relationship management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Customers")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ILogger<CustomersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerDto>>> GetCustomers([FromQuery] CustomerListFilterDto filters)
    {
        try
        {
            _logger.LogInformation("Getting customers with filters");

            // Mock data for now - replace with actual service call using MediatR
            var mockCustomers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@email.com",
                    Phone = "+1-555-0101",
                    Address = "123 Main St, Anytown, USA",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Status = "Active",
                    Segment = "Silver",
                    CreatedAt = DateTime.Now.AddDays(-30),
                    LastPurchaseDate = DateTime.Now.AddDays(-5),
                    TotalSpent = 2300.00m,
                    PurchaseCount = 12,
                    LoyaltyPoints = 2300,
                    FullName = "John Doe",
                    IsLapsed = false,
                    IsHighValue = false,
                    AverageOrderValue = 191.67m
                },
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@email.com",
                    Phone = "+1-555-0102",
                    Address = "456 Oak Ave, Somewhere, USA",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    Status = "Active",
                    Segment = "Gold",
                    CreatedAt = DateTime.Now.AddDays(-45),
                    LastPurchaseDate = DateTime.Now.AddDays(-2),
                    TotalSpent = 8500.00m,
                    PurchaseCount = 28,
                    LoyaltyPoints = 8500,
                    FullName = "Jane Smith",
                    IsLapsed = false,
                    IsHighValue = true,
                    AverageOrderValue = 303.57m
                }
            };

            var result = new PagedResult<CustomerDto>
            {
                Data = mockCustomers,
                TotalCount = mockCustomers.Count,
                Page = filters.Page,
                Limit = filters.Limit,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting customer {CustomerId}", id);

            // Mock data - replace with actual service call using MediatR
            var customer = new CustomerDto
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Phone = "+1-555-0101",
                Address = "123 Main St, Anytown, USA",
                DateOfBirth = new DateTime(1985, 5, 15),
                Status = "Active",
                Segment = "Silver",
                CreatedAt = DateTime.Now.AddDays(-30),
                LastPurchaseDate = DateTime.Now.AddDays(-5),
                TotalSpent = 2300.00m,
                PurchaseCount = 12,
                LoyaltyPoints = 2300,
                FullName = "John Doe",
                IsLapsed = false,
                IsHighValue = false,
                AverageOrderValue = 191.67m
            };

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto request)
    {
        try
        {
            _logger.LogInformation("Creating new customer {Email}", request.Email);

            // Mock response - replace with actual service call using MediatR
            var newCustomer = new CustomerDto
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Status = "Active",
                Segment = "Regular",
                CreatedAt = DateTime.UtcNow,
                LastPurchaseDate = null,
                TotalSpent = 0,
                PurchaseCount = 0,
                LoyaltyPoints = 0,
                FullName = $"{request.FirstName} {request.LastName}",
                IsLapsed = false,
                IsHighValue = false,
                AverageOrderValue = 0
            };

            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto request)
    {
        try
        {
            _logger.LogInformation("Updating customer {CustomerId}", id);

            // Mock response - replace with actual service call using MediatR
            var updatedCustomer = new CustomerDto
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                DateOfBirth = new DateTime(1985, 5, 15), // This wouldn't change in update
                Status = "Active",
                Segment = "Silver",
                CreatedAt = DateTime.Now.AddDays(-30),
                LastPurchaseDate = DateTime.Now.AddDays(-5),
                TotalSpent = 2300.00m,
                PurchaseCount = 12,
                LoyaltyPoints = 2300,
                FullName = $"{request.FirstName} {request.LastName}",
                IsLapsed = false,
                IsHighValue = false,
                AverageOrderValue = 191.67m
            };

            return Ok(updatedCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting customer {CustomerId}", id);

            // Mock response - replace with actual service call using MediatR
            return Ok(new { message = "Customer deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Record a purchase for a customer
    /// </summary>
    [HttpPost("{id}/purchases")]
    public async Task<ActionResult<CustomerDto>> RecordPurchase(Guid id, [FromBody] RecordPurchaseDto request)
    {
        try
        {
            _logger.LogInformation("Recording purchase for customer {CustomerId}: {Amount}", id, request.Amount);

            // Mock response - replace with actual service call using MediatR
            var updatedCustomer = new CustomerDto
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                Phone = "+1-555-0101",
                Address = "123 Main St, Anytown, USA",
                DateOfBirth = new DateTime(1985, 5, 15),
                Status = "Active",
                Segment = "Silver",
                CreatedAt = DateTime.Now.AddDays(-30),
                LastPurchaseDate = DateTime.UtcNow,
                TotalSpent = 2300.00m + request.Amount,
                PurchaseCount = 13,
                LoyaltyPoints = 2300 + (int)request.Amount,
                FullName = "John Doe",
                IsLapsed = false,
                IsHighValue = false,
                AverageOrderValue = (2300.00m + request.Amount) / 13
            };

            return Ok(updatedCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording purchase for customer {CustomerId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get top customers by spending
    /// </summary>
    [HttpGet("top")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetTopCustomers([FromQuery] int count = 10)
    {
        try
        {
            _logger.LogInformation("Getting top {Count} customers", count);

            // Mock data - replace with actual service call using MediatR
            var topCustomers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Premium",
                    LastName = "Customer",
                    Email = "premium@email.com",
                    Phone = "+1-555-9999",
                    Address = "999 Premium St, Elite City, USA",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Status = "Active",
                    Segment = "Premium",
                    CreatedAt = DateTime.Now.AddDays(-365),
                    LastPurchaseDate = DateTime.Now.AddDays(-1),
                    TotalSpent = 15000.00m,
                    PurchaseCount = 45,
                    LoyaltyPoints = 15000,
                    FullName = "Premium Customer",
                    IsLapsed = false,
                    IsHighValue = true,
                    AverageOrderValue = 333.33m
                }
            };

            return Ok(topCustomers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top customers");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get lapsed customers (no purchases in 90+ days)
    /// </summary>
    [HttpGet("lapsed")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetLapsedCustomers()
    {
        try
        {
            _logger.LogInformation("Getting lapsed customers");

            // Mock data - replace with actual service call using MediatR
            var lapsedCustomers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Lapsed",
                    LastName = "Customer",
                    Email = "lapsed@email.com",
                    Phone = "+1-555-0000",
                    Address = "000 Old St, Past City, USA",
                    DateOfBirth = new DateTime(1975, 6, 15),
                    Status = "Active",
                    Segment = "Regular",
                    CreatedAt = DateTime.Now.AddDays(-200),
                    LastPurchaseDate = DateTime.Now.AddDays(-120),
                    TotalSpent = 500.00m,
                    PurchaseCount = 3,
                    LoyaltyPoints = 500,
                    FullName = "Lapsed Customer",
                    IsLapsed = true,
                    IsHighValue = false,
                    AverageOrderValue = 166.67m
                }
            };

            return Ok(lapsedCustomers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lapsed customers");
            return BadRequest(new { error = ex.Message });
        }
    }
}

/// <summary>
/// DTO for recording customer purchases
/// </summary>
public class RecordPurchaseDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
