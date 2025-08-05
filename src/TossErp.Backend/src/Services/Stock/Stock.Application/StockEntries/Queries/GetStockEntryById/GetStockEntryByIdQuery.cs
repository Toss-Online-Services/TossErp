using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Queries.GetStockEntryById;

public class GetStockEntryByIdQuery : IRequest<StockEntryDto>
{
    public Guid Id { get; }
    public GetStockEntryByIdQuery(Guid id) => Id = id;
} 
