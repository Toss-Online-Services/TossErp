using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.StockEntries.Commands.ApproveStockEntry;

public record ApproveStockEntryCommand : IRequest<StockEntryDto>
{
    public Guid Id { get; init; }
    public string? ApprovedBy { get; init; }
    public string? ApprovalNotes { get; init; }
} 
