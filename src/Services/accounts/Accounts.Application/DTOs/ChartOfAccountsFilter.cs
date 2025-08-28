using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Filter for ChartOfAccounts queries
/// </summary>
public class ChartOfAccountsFilter
{
    public AccountType? AccountType { get; set; }
    public Guid? ParentAccountId { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
