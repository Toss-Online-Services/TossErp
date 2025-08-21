using MediatR;
using TossErp.Sales.Application.Common.DTOs;

namespace TossErp.Sales.Application.Queries.GetSaleById;

/// <summary>
/// Query to get sale by ID
/// </summary>
public class GetSaleByIdQuery : IRequest<SaleDto?>
{
    public Guid SaleId { get; set; }

    public GetSaleByIdQuery(Guid saleId)
    {
        SaleId = saleId;
    }
}
