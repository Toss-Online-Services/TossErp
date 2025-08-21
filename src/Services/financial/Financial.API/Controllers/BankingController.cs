using Microsoft.AspNetCore.Mvc;
using Financial.Application.DTOs;

namespace Financial.API.Controllers;

/// <summary>
/// API controller for banking integration and transaction management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Banking")]
public class BankingController : ControllerBase
{
    private readonly ILogger<BankingController> _logger;

    public BankingController(ILogger<BankingController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all linked bank accounts for the current tenant
    /// </summary>
    [HttpGet("accounts")]
    public async Task<ActionResult<IEnumerable<LinkedBankAccountDto>>> GetLinkedAccounts()
    {
        try
        {
            _logger.LogInformation("Getting linked bank accounts");

            // Mock data for now - replace with actual service call
            var mockAccounts = new List<LinkedBankAccountDto>
            {
                new LinkedBankAccountDto
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "****1234",
                    AccountName = "Business Checking",
                    BankName = "First National Bank",
                    AccountType = "Checking",
                    CurrentBalance = 45678.90m,
                    AvailableBalance = 44678.90m,
                    Currency = "USD",
                    LastUpdated = DateTime.Now.AddHours(-2),
                    IsActive = true,
                    IsVerified = true,
                    IsPrimary = true,
                    Provider = "Plaid",
                    ConnectedDate = DateTime.Now.AddDays(-30),
                    LastSyncDate = DateTime.Now.AddHours(-2),
                    RequiresReauth = false,
                    TransactionCount = 156,
                    LastTransactionDate = DateTime.Now.AddDays(-1)
                }
            };

            return Ok(mockAccounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting linked bank accounts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific linked bank account by ID
    /// </summary>
    [HttpGet("accounts/{id}")]
    public async Task<ActionResult<LinkedBankAccountDto>> GetLinkedAccount(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting linked bank account {AccountId}", id);

            // Mock data - replace with actual service call
            var account = new LinkedBankAccountDto
            {
                Id = id,
                AccountNumber = "****1234",
                AccountName = "Business Checking",
                BankName = "First National Bank",
                AccountType = "Checking",
                CurrentBalance = 45678.90m,
                AvailableBalance = 44678.90m,
                Currency = "USD",
                LastUpdated = DateTime.Now.AddHours(-2),
                IsActive = true,
                IsVerified = true,
                IsPrimary = true,
                Provider = "Plaid",
                ConnectedDate = DateTime.Now.AddDays(-30),
                LastSyncDate = DateTime.Now.AddHours(-2),
                RequiresReauth = false,
                TransactionCount = 156,
                LastTransactionDate = DateTime.Now.AddDays(-1)
            };

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting linked bank account {AccountId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Link a new bank account
    /// </summary>
    [HttpPost("accounts")]
    public async Task<ActionResult<LinkedBankAccountDto>> LinkAccount([FromBody] LinkBankAccountDto request)
    {
        try
        {
            _logger.LogInformation("Linking new bank account {BankName} - {AccountName}", request.BankName, request.AccountName);

            // Mock response - replace with actual service call
            var newAccount = new LinkedBankAccountDto
            {
                Id = Guid.NewGuid(),
                AccountNumber = "****" + request.AccountNumber[^4..],
                AccountName = request.AccountName,
                BankName = request.BankName,
                AccountType = request.AccountType,
                CurrentBalance = 0, // Will be updated after first sync
                AvailableBalance = 0,
                Currency = "USD",
                LastUpdated = DateTime.UtcNow,
                IsActive = true,
                IsVerified = false, // Needs verification
                IsPrimary = request.IsPrimary,
                Provider = request.Provider,
                ConnectedDate = DateTime.UtcNow,
                LastSyncDate = null,
                RequiresReauth = false,
                TransactionCount = 0,
                LastTransactionDate = null
            };

            return CreatedAtAction(nameof(GetLinkedAccount), new { id = newAccount.Id }, newAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking bank account");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get transactions for a specific account
    /// </summary>
    [HttpGet("accounts/{id}/transactions")]
    public async Task<ActionResult<IEnumerable<BankTransactionDto>>> GetAccountTransactions(
        Guid id,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? category = null,
        [FromQuery] bool? isReconciled = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Getting transactions for account {AccountId}", id);

            // Mock data - replace with actual service call
            var mockTransactions = new List<BankTransactionDto>
            {
                new BankTransactionDto
                {
                    Id = Guid.NewGuid(),
                    LinkedBankAccountId = id,
                    TransactionId = "TXN123456",
                    Amount = -1250.00m,
                    Currency = "USD",
                    TransactionDate = DateTime.Now.AddDays(-2),
                    PostedDate = DateTime.Now.AddDays(-1),
                    TransactionType = "Debit",
                    Description = "Office supplies purchase",
                    OriginalDescription = "OFFICE DEPOT #1234",
                    MerchantName = "Office Depot",
                    Category = "Office Expenses",
                    Subcategory = "Supplies",
                    RunningBalance = 44428.90m,
                    CounterpartyName = "Office Depot",
                    LocationCity = "Phoenix",
                    IsReconciled = false,
                    IsPending = false,
                    IsBusinessExpense = true,
                    ExpenseCategory = "Office Supplies",
                    AccountName = "Business Checking",
                    BankName = "First National Bank"
                }
            };

            return Ok(mockTransactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for account {AccountId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all transactions across all accounts
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<BankTransactionDto>>> GetAllTransactions(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? category = null,
        [FromQuery] bool? isReconciled = null,
        [FromQuery] bool? isBusinessExpense = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Getting all transactions with filters");

            // Mock data - replace with actual service call
            var mockTransactions = new List<BankTransactionDto>
            {
                new BankTransactionDto
                {
                    Id = Guid.NewGuid(),
                    LinkedBankAccountId = Guid.NewGuid(),
                    TransactionId = "TXN123456",
                    Amount = -1250.00m,
                    Currency = "USD",
                    TransactionDate = DateTime.Now.AddDays(-2),
                    PostedDate = DateTime.Now.AddDays(-1),
                    TransactionType = "Debit",
                    Description = "Office supplies purchase",
                    OriginalDescription = "OFFICE DEPOT #1234",
                    MerchantName = "Office Depot",
                    Category = "Office Expenses",
                    Subcategory = "Supplies",
                    RunningBalance = 44428.90m,
                    CounterpartyName = "Office Depot",
                    LocationCity = "Phoenix",
                    IsReconciled = false,
                    IsPending = false,
                    IsBusinessExpense = true,
                    ExpenseCategory = "Office Supplies",
                    AccountName = "Business Checking",
                    BankName = "First National Bank"
                }
            };

            return Ok(mockTransactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all transactions");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update transaction categorization
    /// </summary>
    [HttpPatch("transactions/{id}/categorize")]
    public async Task<ActionResult> CategorizeTransaction(Guid id, [FromBody] UpdateTransactionCategoryDto request)
    {
        try
        {
            _logger.LogInformation("Updating categorization for transaction {TransactionId}", id);

            // Mock response - replace with actual service call
            return Ok(new { message = "Transaction categorization updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction categorization for {TransactionId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reconcile a transaction
    /// </summary>
    [HttpPatch("transactions/{id}/reconcile")]
    public async Task<ActionResult> ReconcileTransaction(Guid id, [FromBody] ReconcileTransactionDto request)
    {
        try
        {
            _logger.LogInformation("Reconciling transaction {TransactionId}", id);

            // Mock response - replace with actual service call
            return Ok(new { message = "Transaction reconciled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling transaction {TransactionId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Sync transactions for a specific account
    /// </summary>
    [HttpPost("accounts/{id}/sync")]
    public async Task<ActionResult> SyncAccount(Guid id)
    {
        try
        {
            _logger.LogInformation("Syncing account {AccountId}", id);

            // Mock response - replace with actual service call
            return Ok(new { 
                message = "Account sync initiated successfully",
                transactionsImported = 5,
                lastSyncDate = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing account {AccountId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get financial summary across all accounts
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation("Getting financial summary");

            // Mock data - replace with actual service call
            var summary = new FinancialSummaryDto
            {
                TotalBalance = 45678.90m,
                TotalIncome = 12500.00m,
                TotalExpenses = 8750.00m,
                NetCashFlow = 3750.00m,
                TotalTransactions = 156,
                UnreconciledTransactions = 12,
                AccountSummaries = new List<AccountSummaryDto>
                {
                    new AccountSummaryDto
                    {
                        AccountId = Guid.NewGuid(),
                        AccountName = "Business Checking",
                        BankName = "First National Bank",
                        CurrentBalance = 45678.90m,
                        MonthlyIncome = 12500.00m,
                        MonthlyExpenses = 8750.00m,
                        TransactionCount = 156
                    }
                },
                ExpenseCategories = new List<CategorySummaryDto>
                {
                    new CategorySummaryDto
                    {
                        Category = "Office Expenses",
                        Amount = 2500.00m,
                        TransactionCount = 15,
                        Percentage = 28.6m
                    },
                    new CategorySummaryDto
                    {
                        Category = "Travel",
                        Amount = 1800.00m,
                        TransactionCount = 8,
                        Percentage = 20.6m
                    }
                }
            };

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting financial summary");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove/unlink a bank account
    /// </summary>
    [HttpDelete("accounts/{id}")]
    public async Task<ActionResult> UnlinkAccount(Guid id)
    {
        try
        {
            _logger.LogInformation("Unlinking bank account {AccountId}", id);

            // Mock response - replace with actual service call
            return Ok(new { message = "Bank account unlinked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking bank account {AccountId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
