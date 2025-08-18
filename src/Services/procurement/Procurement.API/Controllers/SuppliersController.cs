using MediatR;
using Microsoft.AspNetCore.Mvc;
using TossErp.Procurement.Application.Commands.CreateSupplier;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Queries.GetSuppliers;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.API.Controllers;

/// <summary>
/// API controller for managing suppliers
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all suppliers with optional filtering
    /// </summary>
    /// <param name="status">Filter by status</param>
    /// <param name="name">Filter by name (partial match)</param>
    /// <param name="activeOnly">Show only active suppliers</param>
    /// <returns>List of supplier summaries</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierSummaryDto>>> GetSuppliers(
        [FromQuery] SupplierStatus? status = null,
        [FromQuery] string? name = null,
        [FromQuery] bool activeOnly = false)
    {
        var query = new GetSuppliersQuery
        {
            Status = status,
            Name = name,
            ActiveOnly = activeOnly
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <returns>Supplier details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetSupplier(Guid id)
    {
        // TODO: Implement GetSupplierByIdQuery
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    /// <param name="request">Supplier creation request</param>
    /// <returns>Created supplier</returns>
    [HttpPost]
    public async Task<ActionResult<SupplierDto>> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var command = new CreateSupplierCommand
        {
            Name = request.Name,
            Code = request.Code,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TaxNumber = request.TaxNumber,
            RegistrationNumber = request.RegistrationNumber,
            Notes = request.Notes,
            CreditLimit = request.CreditLimit,
            PaymentTermsDays = request.PaymentTermsDays,
            LeadTimeDays = request.LeadTimeDays
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSupplier), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update supplier contact information
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Contact info update request</param>
    /// <returns>Updated supplier</returns>
    [HttpPut("{id}/contact-info")]
    public async Task<ActionResult<SupplierDto>> UpdateContactInfo(Guid id, [FromBody] UpdateSupplierContactInfoRequest request)
    {
        // TODO: Implement UpdateSupplierContactInfoCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Update supplier business information
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Business info update request</param>
    /// <returns>Updated supplier</returns>
    [HttpPut("{id}/business-info")]
    public async Task<ActionResult<SupplierDto>> UpdateBusinessInfo(Guid id, [FromBody] UpdateSupplierBusinessInfoRequest request)
    {
        // TODO: Implement UpdateSupplierBusinessInfoCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Update supplier financial information
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Financial info update request</param>
    /// <returns>Updated supplier</returns>
    [HttpPut("{id}/financial-info")]
    public async Task<ActionResult<SupplierDto>> UpdateFinancialInfo(Guid id, [FromBody] UpdateSupplierFinancialInfoRequest request)
    {
        // TODO: Implement UpdateSupplierFinancialInfoCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Update supplier operational information
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Operational info update request</param>
    /// <returns>Updated supplier</returns>
    [HttpPut("{id}/operational-info")]
    public async Task<ActionResult<SupplierDto>> UpdateOperationalInfo(Guid id, [FromBody] UpdateSupplierOperationalInfoRequest request)
    {
        // TODO: Implement UpdateSupplierOperationalInfoCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Activate a supplier
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <returns>Updated supplier</returns>
    [HttpPost("{id}/activate")]
    public async Task<ActionResult<SupplierDto>> ActivateSupplier(Guid id)
    {
        // TODO: Implement ActivateSupplierCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Deactivate a supplier
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Deactivation request</param>
    /// <returns>Updated supplier</returns>
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult<SupplierDto>> DeactivateSupplier(Guid id, [FromBody] DeactivateSupplierRequest request)
    {
        // TODO: Implement DeactivateSupplierCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Put a supplier on hold
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Hold request</param>
    /// <returns>Updated supplier</returns>
    [HttpPost("{id}/hold")]
    public async Task<ActionResult<SupplierDto>> PutSupplierOnHold(Guid id, [FromBody] PutSupplierOnHoldRequest request)
    {
        // TODO: Implement PutSupplierOnHoldCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Blacklist a supplier
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Blacklist request</param>
    /// <returns>Updated supplier</returns>
    [HttpPost("{id}/blacklist")]
    public async Task<ActionResult<SupplierDto>> BlacklistSupplier(Guid id, [FromBody] BlacklistSupplierRequest request)
    {
        // TODO: Implement BlacklistSupplierCommand
        return NotFound("Not implemented yet");
    }

    /// <summary>
    /// Add notes to a supplier
    /// </summary>
    /// <param name="id">Supplier ID</param>
    /// <param name="request">Notes request</param>
    /// <returns>Updated supplier</returns>
    [HttpPost("{id}/notes")]
    public async Task<ActionResult<SupplierDto>> AddNotes(Guid id, [FromBody] AddSupplierNotesRequest request)
    {
        // TODO: Implement AddSupplierNotesCommand
        return NotFound("Not implemented yet");
    }
}
