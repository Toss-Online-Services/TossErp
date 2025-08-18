using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Commands.CreatePurchaseOrder;

/// <summary>
/// Command to create a new purchase order
/// </summary>
public class CreatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PaymentTerms PaymentTerms { get; set; } = PaymentTerms.Net30;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}
