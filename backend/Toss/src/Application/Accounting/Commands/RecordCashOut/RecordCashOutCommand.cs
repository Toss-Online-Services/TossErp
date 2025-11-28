using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Commands.RecordCashOut;

public record RecordCashOutCommand : IRequest<int>
{
    public int AccountId { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset EntryDate { get; init; }
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public string? SourceType { get; init; }
    public int? SourceId { get; init; }
    public int? PaymentId { get; init; }
}

public class RecordCashOutCommandHandler : IRequestHandler<RecordCashOutCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public RecordCashOutCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(RecordCashOutCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        if (request.Amount <= 0)
        {
            throw new ValidationException("Amount must be greater than zero.");
        }

        var account = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == request.AccountId && a.BusinessId == _businessContext.CurrentBusinessId,
                cancellationToken);

        if (account == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found.");
        }

        if (!account.IsActive)
        {
            throw new ValidationException("Cannot record entry to an inactive account.");
        }

        // Check if account has sufficient balance (optional - can allow negative for overdraft)
        // For now, we'll allow negative balances but log a warning

        var entry = new CashbookEntry
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            AccountId = request.AccountId,
            Amount = -request.Amount, // Negative for cash out
            EntryDate = request.EntryDate,
            Type = CashbookEntryType.Expense,
            Reference = request.Reference,
            Notes = request.Notes,
            SourceType = request.SourceType,
            SourceId = request.SourceId,
            PaymentId = request.PaymentId
        };

        _context.CashbookEntries.Add(entry);

        // Update account balance atomically
        account.CurrentBalance -= request.Amount;

        await _context.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}

