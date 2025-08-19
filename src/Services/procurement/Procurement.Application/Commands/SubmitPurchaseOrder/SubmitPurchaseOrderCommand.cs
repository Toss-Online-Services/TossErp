using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Commands.SubmitPurchaseOrder;

/// <summary>
/// Command to submit a purchase order for approval
/// </summary>
public class SubmitPurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid PurchaseOrderId { get; set; }
    public string? Notes { get; set; }
}
