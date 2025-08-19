using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Commands.CreateSupplierPriceList;

/// <summary>
/// Command to create a new supplier price list
/// </summary>
public class CreateSupplierPriceListCommand : IRequest<SupplierPriceListDto>
{
    public Guid SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string Currency { get; set; } = "ZAR";
    public List<CreateSupplierPriceListItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request for creating a price list item
/// </summary>
public class CreateSupplierPriceListItemRequest
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSku { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? Notes { get; set; }
}
