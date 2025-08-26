using Crm.Application.Commands;
using Crm.Application.DTOs;
using Crm.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Crm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomersQuery();
            var customers = await _mediator.Send(query, cancellationToken);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return StatusCode(500, "An error occurred while retrieving customers");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerByIdQuery(id);
            var customer = await _mediator.Send(query, cancellationToken);
            
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");
                
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while retrieving the customer");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var customerId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCustomer), new { id = customerId }, customerId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create customer: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "An error occurred while creating the customer");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("Customer ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update customer {CustomerId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while updating the customer");
        }
    }

    [HttpPost("{id:guid}/purchase")]
    public async Task<ActionResult> RecordPurchase(Guid id, [FromBody] RecordPurchaseCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (id != command.CustomerId)
                return BadRequest("Customer ID in URL does not match the ID in the request body");
                
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to record purchase for customer {CustomerId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording purchase for customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while recording the purchase");
        }
    }

    [HttpGet("top")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetTopCustomers([FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetTopCustomersQuery(count);
            var customers = await _mediator.Send(query, cancellationToken);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top customers");
            return StatusCode(500, "An error occurred while retrieving top customers");
        }
    }

    [HttpGet("lapsed")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetLapsedCustomers([FromQuery] int daysThreshold = 90, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetLapsedCustomersQuery(daysThreshold);
            var customers = await _mediator.Send(query, cancellationToken);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lapsed customers");
            return StatusCode(500, "An error occurred while retrieving lapsed customers");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");
                
            var query = new GetCustomersQuery { SearchTerm = searchTerm };
            var customers = await _mediator.Send(query, cancellationToken);
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
            return StatusCode(500, "An error occurred while searching customers");
        }
    }
}