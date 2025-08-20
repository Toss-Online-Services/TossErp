using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Queries.GetAccounts;

/// <summary>
/// Handler for GetAccountsQuery
/// </summary>
public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, IEnumerable<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<GetAccountsQueryHandler> _logger;

    public GetAccountsQueryHandler(
        IAccountRepository accountRepository,
        ILogger<GetAccountsQueryHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving accounts with filters: Type={Type}, Category={Category}, IsActive={IsActive}", 
                request.Type, request.Category, request.IsActive);

            // Get all accounts for the tenant
            var accounts = await _accountRepository.GetAllAsync("tenant-id", cancellationToken);

            // Apply filters
            var filteredAccounts = accounts.AsEnumerable();

            if (request.Type.HasValue)
            {
                filteredAccounts = filteredAccounts.Where(a => a.Type == request.Type.Value);
            }

            if (request.Category.HasValue)
            {
                filteredAccounts = filteredAccounts.Where(a => a.Category == request.Category.Value);
            }

            if (request.IsActive.HasValue)
            {
                filteredAccounts = filteredAccounts.Where(a => a.IsActive == request.IsActive.Value);
            }

            // Convert to DTOs
            var accountDtos = filteredAccounts.Select(account => new AccountDto
            {
                Id = account.Id,
                Code = account.Code,
                Name = account.Name,
                Description = account.Description,
                Type = account.Type,
                Category = account.Category,
                IsActive = account.IsActive,
                OpeningBalance = account.OpeningBalance.Amount,
                Currency = account.OpeningBalance.Currency,
                OpeningBalanceDate = account.OpeningBalanceDate,
                CurrentBalance = account.GetCurrentBalance().Amount,
                DisplayName = account.DisplayName
            }).ToList();

            _logger.LogInformation("Retrieved {Count} accounts", accountDtos.Count);

            return accountDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts");
            throw;
        }
    }
}


