using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Domain.Enums;

namespace TossErp.Procurement.Application.Queries.GetPurchaseOrders;

/// <summary>
/// Query to get purchase orders with optional filtering
/// </summary>
public class GetPurchaseOrdersQuery : IRequest<IEnumerable<PurchaseOrderSummaryDto>>
{
    public PurchaseOrderStatus? Status { get; set; }
    public Guid? SupplierId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IncludeOverdue { get; set; } = false;
}
