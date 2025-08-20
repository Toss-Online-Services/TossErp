using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Commands.CreateAccount;

/// <summary>
/// Handler for CreateAccountCommand
/// </summary>
public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAccountCommandHandler> _logger;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAccountCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new account with code {Code} and name {Name}", request.Code, request.Name);

            // Check if account with same code already exists
            var existingAccount = await _accountRepository.GetByCodeAsync(request.Code, "tenant-id", cancellationToken);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Account with code '{request.Code}' already exists");
            }

            // Create the account
            var openingBalance = new Money(request.OpeningBalance, request.Currency);
            var account = Account.Create(
                request.Code,
                request.Name,
                request.Type,
                request.Category,
                "tenant-id", // TODO: Get from current user context
                openingBalance,
                request.Description
            );

            // Save the account
            account = await _accountRepository.AddAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created account {AccountId} with code {Code}", account.Id, account.Code);

            return account.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account with code {Code}", request.Code);
            throw;
        }
    }
}


