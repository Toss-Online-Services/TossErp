using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Domain.Common;

namespace TossErp.Procurement.Application.Queries.GetPurchaseOrders;

/// <summary>
/// Handler for GetPurchaseOrdersQuery
/// </summary>
public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, IEnumerable<PurchaseOrderSummaryDto>>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository purchaseOrderRepository)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
    }

    public async Task<IEnumerable<PurchaseOrderSummaryDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.PurchaseOrder> purchaseOrders;

        // Apply filters based on request
        if (request.Status.HasValue)
        {
            purchaseOrders = await _purchaseOrderRepository.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else if (request.SupplierId.HasValue)
        {
            purchaseOrders = await _purchaseOrderRepository.GetBySupplierAsync(request.SupplierId.Value, cancellationToken);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            purchaseOrders = await _purchaseOrderRepository.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else if (request.IncludeOverdue)
        {
            purchaseOrders = await _purchaseOrderRepository.GetOverdueAsync(cancellationToken);
        }
        else
        {
            // Get all purchase orders
            purchaseOrders = await _purchaseOrderRepository.GetAllAsync(cancellationToken);
        }

        // Map to summary DTOs
        return purchaseOrders.Select(MapToSummaryDto);
    }

    private static PurchaseOrderSummaryDto MapToSummaryDto(Domain.Entities.PurchaseOrder purchaseOrder)
    {
        var isOverdue = purchaseOrder.ExpectedDeliveryDate.HasValue && 
                       purchaseOrder.ExpectedDeliveryDate.Value < DateTime.UtcNow && 
                       purchaseOrder.Status != Domain.Enums.PurchaseOrderStatus.Received &&
                       purchaseOrder.Status != Domain.Enums.PurchaseOrderStatus.Cancelled;

        return new PurchaseOrderSummaryDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber.Value,
            SupplierName = purchaseOrder.SupplierName,
            Status = purchaseOrder.Status,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            TotalAmount = purchaseOrder.TotalAmount,
            IsOverdue = isOverdue
        };
    }
}
