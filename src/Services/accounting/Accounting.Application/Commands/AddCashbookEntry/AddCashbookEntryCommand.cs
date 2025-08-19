using MediatR;
using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Application.Commands.AddCashbookEntry;

/// <summary>
/// Command to add a new cashbook entry
/// </summary>
public class AddCashbookEntryCommand : IRequest<CashbookEntryDto>
{
    public Guid CashbookId { get; init; }
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Reference { get; init; }
    public DateTime TransactionDate { get; init; } = DateTime.Today;
    public string? RelatedEntityId { get; init; }
    public string? RelatedEntityType { get; init; }
}
