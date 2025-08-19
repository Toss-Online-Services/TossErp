using MediatR;
using Microsoft.AspNetCore.Mvc;
using TossErp.Procurement.Application.Commands.CreatePurchaseOrder;
using TossErp.Procurement.Application.Commands.SubmitPurchaseOrder;
using TossErp.Procurement.Application.Commands.ReceivePurchaseOrder;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Queries.GetPurchaseOrders;
using TossErp.Procurement.Application.Queries.GetPurchaseOrderById;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.API.Controllers;

/// <summary>
/// API controller for managing purchase orders
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all purchase orders with optional filtering
    /// </summary>
    /// <param name="status">Filter by status</param>
    /// <param name="supplierId">Filter by supplier ID</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="includeOverdue">Include overdue purchase orders</param>
    /// <returns>List of purchase order summaries</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseOrderSummaryDto>>> GetPurchaseOrders(
        [FromQuery] PurchaseOrderStatus? status = null,
        [FromQuery] Guid? supplierId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool includeOverdue = false)
    {
        var query = new GetPurchaseOrdersQuery
        {
            Status = status,
            SupplierId = supplierId,
            StartDate = startDate,
            EndDate = endDate,
            IncludeOverdue = includeOverdue
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <returns>Purchase order details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetPurchaseOrder(Guid id)
    {
        var query = new GetPurchaseOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound($"Purchase order with ID {id} not found");
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new purchase order
    /// </summary>
    /// <param name="request">Purchase order creation request</param>
    /// <returns>Created purchase order</returns>
    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> CreatePurchaseOrder([FromBody] CreatePurchaseOrderRequest request)
    {
        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = request.SupplierId,
            SupplierName = request.SupplierName,
            PaymentTerms = request.PaymentTerms,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Notes = request.Notes,
            Items = request.Items
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPurchaseOrder), new { id = result.Id }, result);
    }

    /// <summary>
    /// Submit a purchase order for approval
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Submit request with optional notes</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<PurchaseOrderDto>> SubmitPurchaseOrder(Guid id, [FromBody] SubmitPurchaseOrderRequest? request = null)
    {
        var command = new SubmitPurchaseOrderCommand
        {
            PurchaseOrderId = id,
            Notes = request?.Notes
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Approve a purchase order
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Approval request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/approve")]
    public Task<ActionResult<PurchaseOrderDto>> ApprovePurchaseOrder(Guid id, [FromBody] ApprovePurchaseOrderRequest request)
    {
        // TODO: Implement ApprovePurchaseOrderCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Send a purchase order to supplier
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/send")]
    public Task<ActionResult<PurchaseOrderDto>> SendPurchaseOrder(Guid id)
    {
        // TODO: Implement SendPurchaseOrderCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Acknowledge receipt from supplier
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/acknowledge")]
    public Task<ActionResult<PurchaseOrderDto>> AcknowledgePurchaseOrder(Guid id)
    {
        // TODO: Implement AcknowledgePurchaseOrderCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Receive items from supplier
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Receive items request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/receive")]
    public async Task<ActionResult<PurchaseOrderDto>> ReceiveItems(Guid id, [FromBody] ReceivePurchaseOrderRequest request)
    {
        var command = new ReceivePurchaseOrderCommand
        {
            PurchaseOrderId = id,
            ReceivedDate = request.ReceivedDate,
            ReceiptNumber = request.ReceiptNumber,
            Notes = request.Notes,
            Items = request.Items.Select(item => new TossErp.Procurement.Application.Commands.ReceivePurchaseOrder.ReceivePurchaseOrderItemRequest
            {
                PurchaseOrderItemId = item.PurchaseOrderItemId,
                ReceivedQuantity = item.ReceivedQuantity,
                UnitPrice = item.UnitPrice,
                Notes = item.Notes
            }).ToList()
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a purchase order
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Cancellation request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/cancel")]
    public Task<ActionResult<PurchaseOrderDto>> CancelPurchaseOrder(Guid id, [FromBody] CancelPurchaseOrderRequest request)
    {
        // TODO: Implement CancelPurchaseOrderCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Put a purchase order on hold
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Hold request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/hold")]
    public Task<ActionResult<PurchaseOrderDto>> PutPurchaseOrderOnHold(Guid id, [FromBody] PutPurchaseOrderOnHoldRequest request)
    {
        // TODO: Implement PutPurchaseOrderOnHoldCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Resume a purchase order from hold
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/resume")]
    public Task<ActionResult<PurchaseOrderDto>> ResumePurchaseOrderFromHold(Guid id)
    {
        // TODO: Implement ResumePurchaseOrderFromHoldCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Update purchase order delivery date
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Delivery date update request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPut("{id}/delivery-date")]
    public Task<ActionResult<PurchaseOrderDto>> UpdateDeliveryDate(Guid id, [FromBody] UpdatePurchaseOrderDeliveryDateRequest request)
    {
        // TODO: Implement UpdatePurchaseOrderDeliveryDateCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }

    /// <summary>
    /// Add notes to a purchase order
    /// </summary>
    /// <param name="id">Purchase order ID</param>
    /// <param name="request">Notes request</param>
    /// <returns>Updated purchase order</returns>
    [HttpPost("{id}/notes")]
    public Task<ActionResult<PurchaseOrderDto>> AddNotes(Guid id, [FromBody] AddPurchaseOrderNotesRequest request)
    {
        // TODO: Implement AddPurchaseOrderNotesCommand
        return Task.FromResult<ActionResult<PurchaseOrderDto>>(NotFound("Not implemented yet"));
    }
}
