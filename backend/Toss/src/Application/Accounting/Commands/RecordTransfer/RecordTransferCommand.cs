using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Commands.RecordTransfer;

public record RecordTransferCommand : IRequest<int>
{
    public int FromAccountId { get; init; }
    public int ToAccountId { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset EntryDate { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
}

public class RecordTransferCommandHandler : IRequestHandler<RecordTransferCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public RecordTransferCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(RecordTransferCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.Amount <= 0)
        {
            throw new ValidationException("Amount must be greater than zero.");
        }

        if (request.FromAccountId == request.ToAccountId)
        {
            throw new ValidationException("Cannot transfer to the same account.");
        }

        var fromAccount = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == request.FromAccountId && a.BusinessId == _businessContext.CurrentBusinessId,
                cancellationToken);

        if (fromAccount == null)
        {
            throw new NotFoundException($"Source account with ID {request.FromAccountId} not found.");
        }

        var toAccount = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == request.ToAccountId && a.BusinessId == _businessContext.CurrentBusinessId,
                cancellationToken);

        if (toAccount == null)
        {
            throw new NotFoundException($"Destination account with ID {request.ToAccountId} not found.");
        }

        if (!fromAccount.IsActive || !toAccount.IsActive)
        {
            throw new ValidationException("Cannot transfer to or from an inactive account.");
        }

        // Create two entries: one negative (from) and one positive (to)
        var fromEntry = new CashbookEntry
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            AccountId = request.FromAccountId,
            Amount = -request.Amount,
            EntryDate = request.EntryDate,
            Type = CashbookEntryType.Transfer,
            Reference = request.Reference ?? $"Transfer to {toAccount.Name}",
            Notes = request.Notes,
            CounterpartyAccountId = request.ToAccountId
        };

        var toEntry = new CashbookEntry
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            AccountId = request.ToAccountId,
            Amount = request.Amount,
            EntryDate = request.EntryDate,
            Type = CashbookEntryType.Transfer,
            Reference = request.Reference ?? $"Transfer from {fromAccount.Name}",
            Notes = request.Notes,
            CounterpartyAccountId = request.FromAccountId
        };

        _context.CashbookEntries.AddRange(fromEntry, toEntry);

        // Update both account balances atomically
        fromAccount.CurrentBalance -= request.Amount;
        toAccount.CurrentBalance += request.Amount;

        await _context.SaveChangesAsync(cancellationToken);

        return fromEntry.Id; // Return the from entry ID as the primary transaction ID
    }
}

