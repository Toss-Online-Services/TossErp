using MediatR;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.CreateCashbookEntry;

/// <summary>
/// Command to create a new cashbook entry
/// </summary>
public class CreateCashbookEntryCommand : IRequest<Guid>
{
    public DateTime TransactionDate { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "ZAR";
    public EntryType Type { get; init; }
    public EntryCategory Category { get; init; }
    public Guid AccountId { get; init; }
    public string? RelatedEntityId { get; init; }
    public string? RelatedEntityType { get; init; }
}

