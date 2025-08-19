using MediatR;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Queries.GetAccounts;

/// <summary>
/// Query to get accounts with optional filtering
/// </summary>
public class GetAccountsQuery : IRequest<IEnumerable<AccountDto>>
{
    public AccountType? Type { get; init; }
    public AccountCategory? Category { get; init; }
    public bool? IsActive { get; init; }
}

