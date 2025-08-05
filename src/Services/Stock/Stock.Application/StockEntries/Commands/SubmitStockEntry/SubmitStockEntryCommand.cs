using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Commands.SubmitStockEntry;

public record SubmitStockEntryCommand : IRequest<StockEntryDto>
{
    public Guid Id { get; init; }
} 
