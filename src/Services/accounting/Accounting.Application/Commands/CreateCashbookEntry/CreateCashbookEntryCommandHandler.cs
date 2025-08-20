using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.CreateCashbookEntry;

/// <summary>
/// Handler for CreateCashbookEntryCommand
/// </summary>
public class CreateCashbookEntryCommandHandler : IRequestHandler<CreateCashbookEntryCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICashbookRepository _cashbookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCashbookEntryCommandHandler> _logger;

    public CreateCashbookEntryCommandHandler(
        IAccountRepository accountRepository,
        ICashbookRepository cashbookRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCashbookEntryCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _cashbookRepository = cashbookRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCashbookEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new cashbook entry with reference {Reference} and amount {Amount}", request.Reference, request.Amount);

            // Validate that the account exists
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                throw new InvalidOperationException($"Account with ID '{request.AccountId}' not found");
            }

            // Get or create default cashbook
            var cashbook = await GetOrCreateDefaultCashbookAsync("tenant-id", cancellationToken);

            // Create the entry
            var amount = new Money(request.Amount, request.Currency);
            var entry = cashbook.CreateAndAddEntry(
                request.TransactionDate,
                request.Reference,
                request.Description,
                amount,
                request.Type,
                request.Category,
                request.AccountId,
                request.RelatedEntityId,
                request.RelatedEntityType
            );

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created cashbook entry {EntryId} with reference {Reference}", entry.Id, entry.Reference);

            return entry.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cashbook entry with reference {Reference}", request.Reference);
            throw;
        }
    }

    private async Task<Cashbook> GetOrCreateDefaultCashbookAsync(string tenantId, CancellationToken cancellationToken)
    {
        var cashbook = await _cashbookRepository.GetByNameAsync("Main Cashbook", tenantId, cancellationToken);
        
        if (cashbook == null)
        {
            cashbook = Cashbook.Create("Main Cashbook", tenantId, Money.Zero(), "Default cashbook for the tenant");
            cashbook = await _cashbookRepository.AddAsync(cashbook, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return cashbook;
    }
}


