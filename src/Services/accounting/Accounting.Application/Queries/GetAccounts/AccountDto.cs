using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Application.Queries.GetAccounts;

/// <summary>
/// DTO for account data
/// </summary>
public class AccountDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public AccountType Type { get; init; }
    public AccountCategory Category { get; init; }
    public bool IsActive { get; init; }
    public decimal OpeningBalance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime OpeningBalanceDate { get; init; }
    public decimal CurrentBalance { get; init; }
    public string DisplayName { get; init; } = string.Empty;
}

