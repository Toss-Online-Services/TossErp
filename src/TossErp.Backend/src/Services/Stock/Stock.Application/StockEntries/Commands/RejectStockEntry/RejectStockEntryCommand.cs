using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Commands.RejectStockEntry;

public record RejectStockEntryCommand : IRequest<StockEntryDto>
{
    public Guid Id { get; init; }
    public string? RejectedBy { get; init; }
    public string? RejectionReason { get; init; }
} 
