using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Queries.GetPurchaseOrderById;

/// <summary>
/// Query to get a specific purchase order by ID
/// </summary>
public class GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderDto?>
{
    public Guid Id { get; set; }
}
