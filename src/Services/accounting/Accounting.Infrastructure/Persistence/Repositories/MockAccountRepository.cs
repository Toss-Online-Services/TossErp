using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Domain.Common;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;

namespace TossErp.Accounting.Infrastructure.Persistence.Repositories;

/// <summary>
/// Mock implementation of IAccountRepository for testing
/// </summary>
public class MockAccountRepository : IAccountRepository
{
    private readonly List<Account> _accounts = new();

    public MockAccountRepository()
    {
        // Initialize with some default accounts
        InitializeDefaultAccounts();
    }

    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(account);
    }

    public Task<Account?> GetByCodeAsync(string code, string tenantId, CancellationToken cancellationToken = default)
    {
        var account = _accounts.FirstOrDefault(a => a.Code == code && a.TenantId == tenantId);
        return Task.FromResult(account);
    }

    public Task<IEnumerable<Account>> GetByTypeAsync(AccountType type, string tenantId, CancellationToken cancellationToken = default)
    {
        var accounts = _accounts.Where(a => a.Type == type && a.TenantId == tenantId);
        return Task.FromResult(accounts);
    }

    public Task<IEnumerable<Account>> GetByCategoryAsync(AccountCategory category, string tenantId, CancellationToken cancellationToken = default)
    {
        var accounts = _accounts.Where(a => a.Category == category && a.TenantId == tenantId);
        return Task.FromResult(accounts);
    }

    public Task<IEnumerable<Account>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var accounts = _accounts.Where(a => a.TenantId == tenantId);
        return Task.FromResult(accounts);
    }

    public Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        _accounts.Add(account);
        return Task.FromResult(account);
    }

    public Task<Account> UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        var existingIndex = _accounts.FindIndex(a => a.Id == account.Id);
        if (existingIndex >= 0)
        {
            _accounts[existingIndex] = account;
        }
        return Task.FromResult(account);
    }

    public Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        _accounts.RemoveAll(a => a.Id == account.Id);
        return Task.CompletedTask;
    }

    private void InitializeDefaultAccounts()
    {
        var tenantId = "tenant-id";

        // Asset accounts
        _accounts.Add(Account.Create("1000", "Cash", AccountType.Asset, AccountCategory.Cash, tenantId, new Money(0)));
        _accounts.Add(Account.Create("1001", "Bank Account", AccountType.Asset, AccountCategory.Bank, tenantId, new Money(0)));
        _accounts.Add(Account.Create("1002", "Accounts Receivable", AccountType.Asset, AccountCategory.AccountsReceivable, tenantId, new Money(0)));
        _accounts.Add(Account.Create("1003", "Inventory", AccountType.Asset, AccountCategory.Inventory, tenantId, new Money(0)));

        // Liability accounts
        _accounts.Add(Account.Create("2000", "Accounts Payable", AccountType.Liability, AccountCategory.AccountsPayable, tenantId, new Money(0)));
        _accounts.Add(Account.Create("2001", "Loans", AccountType.Liability, AccountCategory.Loans, tenantId, new Money(0)));
        _accounts.Add(Account.Create("2002", "Purchase Tax Payable", AccountType.Liability, AccountCategory.AccruedExpenses, tenantId, new Money(0)));

        // Equity accounts
        _accounts.Add(Account.Create("3000", "Owner Equity", AccountType.Equity, AccountCategory.OwnerEquity, tenantId, new Money(0)));
        _accounts.Add(Account.Create("3001", "Retained Earnings", AccountType.Equity, AccountCategory.RetainedEarnings, tenantId, new Money(0)));

        // Revenue accounts
        _accounts.Add(Account.Create("4000", "Sales", AccountType.Revenue, AccountCategory.Sales, tenantId, new Money(0)));
        _accounts.Add(Account.Create("4001", "Service Revenue", AccountType.Revenue, AccountCategory.ServiceRevenue, tenantId, new Money(0)));
        _accounts.Add(Account.Create("4002", "Sales Tax Payable", AccountType.Liability, AccountCategory.AccruedExpenses, tenantId, new Money(0)));

        // Expense accounts
        _accounts.Add(Account.Create("5000", "Cost of Goods Sold", AccountType.Expense, AccountCategory.CostOfGoodsSold, tenantId, new Money(0)));
        _accounts.Add(Account.Create("5001", "Operating Expenses", AccountType.Expense, AccountCategory.OperatingExpenses, tenantId, new Money(0)));
        _accounts.Add(Account.Create("5008", "Other Expenses", AccountType.Expense, AccountCategory.OtherExpenses, tenantId, new Money(0)));
    }
}


