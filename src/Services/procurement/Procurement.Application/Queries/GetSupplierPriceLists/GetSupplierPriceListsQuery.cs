using MediatR;
using TossErp.Procurement.Application.Common.DTOs;

namespace TossErp.Procurement.Application.Queries.GetSupplierPriceLists;

/// <summary>
/// Query to get price lists for a specific supplier
/// </summary>
public class GetSupplierPriceListsQuery : IRequest<IEnumerable<SupplierPriceListDto>>
{
    public Guid SupplierId { get; set; }
    public bool ActiveOnly { get; set; } = false;
}
