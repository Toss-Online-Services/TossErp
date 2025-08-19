using MediatR;
using TossErp.Procurement.Application.Common.Interfaces;

namespace TossErp.Procurement.Application.Queries.GetItemPrice;

/// <summary>
/// Handler for GetItemPriceQuery
/// </summary>
public class GetItemPriceQueryHandler : IRequestHandler<GetItemPriceQuery, ItemPriceResult?>
{
    private readonly ISupplierPriceListRepository _priceListRepository;

    public GetItemPriceQueryHandler(ISupplierPriceListRepository priceListRepository)
    {
        _priceListRepository = priceListRepository;
    }

    public async Task<ItemPriceResult?> Handle(GetItemPriceQuery request, CancellationToken cancellationToken)
    {
        // Get the current effective price list for the supplier
        var priceList = await _priceListRepository.GetCurrentEffectiveBySupplierIdAsync(request.SupplierId, cancellationToken);
        if (priceList == null)
            return null;

        // Get the item price from the price list
        var priceListItem = priceList.GetItemPrice(request.ItemId);
        if (priceListItem == null)
            return null;

        return new ItemPriceResult
        {
            UnitPrice = priceListItem.UnitPrice,
            Currency = priceListItem.Currency,
            MinimumOrderQuantity = priceListItem.MinimumOrderQuantity,
            LeadTimeDays = priceListItem.LeadTimeDays,
            Notes = priceListItem.Notes,
            LastUpdated = priceListItem.LastUpdated,
            PriceListName = priceList.Name
        };
    }
}
