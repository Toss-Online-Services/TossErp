using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Commands.ReverseStockEntry;

public record ReverseStockEntryCommand : IRequest<StockEntryDto>
{
    public Guid Id { get; init; }
    public string? Reason { get; init; }
    public string? ReversedBy { get; init; }
} 
