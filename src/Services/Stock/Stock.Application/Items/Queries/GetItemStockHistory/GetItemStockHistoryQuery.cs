using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItemStockHistory;

public record GetItemStockHistoryQuery : IRequest<List<StockLedgerEntryDto>>
{
    public Guid ItemId { get; init; }
} 
