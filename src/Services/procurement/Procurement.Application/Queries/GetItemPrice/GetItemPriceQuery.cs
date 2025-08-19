using MediatR;

namespace TossErp.Procurement.Application.Queries.GetItemPrice;

/// <summary>
/// Query to get the current price and lead time for an item from a supplier
/// </summary>
public class GetItemPriceQuery : IRequest<ItemPriceResult?>
{
    public Guid SupplierId { get; set; }
    public Guid ItemId { get; set; }
}

/// <summary>
/// Result containing item price and lead time information
/// </summary>
public class ItemPriceResult
{
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public decimal? MinimumOrderQuantity { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? Notes { get; set; }
    public DateTime LastUpdated { get; set; }
    public string PriceListName { get; set; } = string.Empty;
}
