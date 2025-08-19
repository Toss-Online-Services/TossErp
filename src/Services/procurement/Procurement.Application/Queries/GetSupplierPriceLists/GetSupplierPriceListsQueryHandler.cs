using MediatR;
using TossErp.Procurement.Application.Common.DTOs;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Entities;

namespace TossErp.Procurement.Application.Queries.GetSupplierPriceLists;

/// <summary>
/// Handler for GetSupplierPriceListsQuery
/// </summary>
public class GetSupplierPriceListsQueryHandler : IRequestHandler<GetSupplierPriceListsQuery, IEnumerable<SupplierPriceListDto>>
{
    private readonly ISupplierPriceListRepository _priceListRepository;

    public GetSupplierPriceListsQueryHandler(ISupplierPriceListRepository priceListRepository)
    {
        _priceListRepository = priceListRepository;
    }

    public async Task<IEnumerable<SupplierPriceListDto>> Handle(GetSupplierPriceListsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SupplierPriceList> priceLists;

        if (request.ActiveOnly)
        {
            priceLists = await _priceListRepository.GetActiveBySupplierIdAsync(request.SupplierId, cancellationToken);
        }
        else
        {
            priceLists = await _priceListRepository.GetBySupplierIdAsync(request.SupplierId, cancellationToken);
        }

        return priceLists.Select(MapToDto);
    }

    private static SupplierPriceListDto MapToDto(SupplierPriceList priceList)
    {
        return new SupplierPriceListDto
        {
            Id = priceList.Id,
            SupplierId = priceList.SupplierId,
            SupplierName = priceList.SupplierName,
            Name = priceList.Name,
            Description = priceList.Description,
            EffectiveFrom = priceList.EffectiveFrom,
            EffectiveTo = priceList.EffectiveTo,
            IsActive = priceList.IsActive,
            Currency = priceList.Currency,
            IsCurrentlyEffective = priceList.IsCurrentlyEffective,
            Items = priceList.Items.Select(MapItemToDto).ToList(),
            CreatedAt = priceList.CreatedAt,
            UpdatedAt = priceList.UpdatedAt,
            CreatedBy = priceList.CreatedBy,
            UpdatedBy = priceList.UpdatedBy,
            TenantId = priceList.TenantId
        };
    }

    private static SupplierPriceListItemDto MapItemToDto(SupplierPriceListItem item)
    {
        return new SupplierPriceListItemDto
        {
            Id = item.Id,
            SupplierPriceListId = item.SupplierPriceListId,
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemSku = item.ItemSku,
            UnitPrice = item.UnitPrice,
            Currency = item.Currency,
            MinimumOrderQuantity = item.MinimumOrderQuantity,
            LeadTimeDays = item.LeadTimeDays,
            Notes = item.Notes,
            LastUpdated = item.LastUpdated
        };
    }
}
