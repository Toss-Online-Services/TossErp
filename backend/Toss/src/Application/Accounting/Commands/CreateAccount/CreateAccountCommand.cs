using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Enums;

namespace Toss.Application.Accounting.Commands.CreateAccount;

public record CreateAccountCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public AccountType Type { get; init; }
    public int? StoreId { get; init; }
    public string? AccountNumber { get; init; }
    public string? BankName { get; init; }
    public string? Notes { get; init; }
    public decimal InitialBalance { get; init; }
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public CreateAccountCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Check for duplicate name within the business
        var existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.BusinessId == _businessContext.CurrentBusinessId && a.Name == request.Name,
                cancellationToken);

        if (existingAccount != null)
        {
            throw new ValidationException($"An account with the name '{request.Name}' already exists.");
        }

        var account = new Account
        {
            BusinessId = _businessContext.CurrentBusinessId!.Value,
            Name = request.Name,
            Type = request.Type,
            StoreId = request.StoreId,
            AccountNumber = request.AccountNumber,
            BankName = request.BankName,
            Notes = request.Notes,
            CurrentBalance = request.InitialBalance,
            IsActive = true
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        // If initial balance is provided, create an opening entry
        if (request.InitialBalance != 0)
        {
            var openingEntry = new CashbookEntry
            {
                BusinessId = _businessContext.CurrentBusinessId!.Value,
                AccountId = account.Id,
                Amount = request.InitialBalance,
                EntryDate = DateTimeOffset.UtcNow,
                Type = CashbookEntryType.Expense, // Opening balance
                Reference = "Opening Balance",
                Notes = "Initial account balance"
            };

            _context.CashbookEntries.Add(openingEntry);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return account.Id;
    }
}

