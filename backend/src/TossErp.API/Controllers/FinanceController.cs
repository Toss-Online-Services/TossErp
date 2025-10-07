using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TossErp.Domain.Entities.Finance;
using TossErp.Infrastructure.Data;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FinanceController> _logger;

    public FinanceController(ApplicationDbContext context, ILogger<FinanceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Accounts

    /// <summary>
    /// Get chart of accounts
    /// </summary>
    [HttpGet("accounts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccounts(
        [FromQuery] AccountType? type = null,
        [FromQuery] bool? isActive = null)
    {
        var query = _context.Accounts
            .Include(a => a.SubAccounts)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        var accounts = await query
            .Where(a => a.ParentAccountId == null) // Get top-level accounts
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        return Ok(accounts);
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("accounts/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Account>> GetAccount(int id)
    {
        var account = await _context.Accounts
            .Include(a => a.SubAccounts)
            .Include(a => a.ParentAccount)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (account == null)
            return NotFound();

        return Ok(account);
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [HttpPost("accounts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            // Check if account code already exists
            if (await _context.Accounts.AnyAsync(a => a.AccountCode == request.AccountCode))
                return BadRequest(new { error = "Account code already exists" });

            var account = new Account
            {
                AccountCode = request.AccountCode,
                Name = request.Name,
                Type = request.Type,
                Description = request.Description,
                ParentAccountId = request.ParentAccountId,
                Currency = request.Currency ?? "ZAR",
                IsActive = true,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created account {AccountCode} - {AccountName}", account.AccountCode, account.Name);

            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return BadRequest(new { error = "Failed to create account", message = ex.Message });
        }
    }

    #endregion

    #region Journal Entries

    /// <summary>
    /// Get journal entries
    /// </summary>
    [HttpGet("journal-entries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournalEntries(
        [FromQuery] JournalEntryStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.JournalEntries
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(j => j.EntryDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(j => j.EntryDate <= endDate.Value);

        var entries = await query
            .OrderByDescending(j => j.EntryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(entries);
    }

    /// <summary>
    /// Get journal entry by ID
    /// </summary>
    [HttpGet("journal-entries/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JournalEntry>> GetJournalEntry(int id)
    {
        var entry = await _context.JournalEntries
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (entry == null)
            return NotFound();

        return Ok(entry);
    }

    /// <summary>
    /// Create a new journal entry
    /// </summary>
    [HttpPost("journal-entries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JournalEntry>> CreateJournalEntry([FromBody] CreateJournalEntryRequest request)
    {
        try
        {
            var entry = new JournalEntry
            {
                EntryNumber = $"JE-{DateTime.UtcNow:yyyyMMddHHmmss}",
                EntryDate = request.EntryDate,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId,
                ReferenceNumber = request.ReferenceNumber,
                Description = request.Description,
                Notes = request.Notes,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            int lineNumber = 1;
            foreach (var lineRequest in request.Lines)
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    AccountId = lineRequest.AccountId,
                    DebitAmount = lineRequest.DebitAmount,
                    CreditAmount = lineRequest.CreditAmount,
                    Description = lineRequest.Description,
                    LineNumber = lineNumber++,
                    CreatedBy = User.Identity?.Name ?? "System"
                });
            }

            // Validate balance before saving
            if (!entry.IsBalanced())
                return BadRequest(new { error = "Journal entry is not balanced. Debits must equal credits." });

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created journal entry {EntryNumber}", entry.EntryNumber);

            return CreatedAtAction(nameof(GetJournalEntry), new { id = entry.Id }, entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating journal entry");
            return BadRequest(new { error = "Failed to create journal entry", message = ex.Message });
        }
    }

    /// <summary>
    /// Post a journal entry (finalize it)
    /// </summary>
    [HttpPost("journal-entries/{id}/post")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostJournalEntry(int id)
    {
        var entry = await _context.JournalEntries
            .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (entry == null)
            return NotFound();

        try
        {
            entry.Post(User.Identity?.Name ?? "System");

            // Update account balances
            foreach (var line in entry.Lines)
            {
                if (line.DebitAmount > 0)
                    line.Account.Debit(line.DebitAmount);
                if (line.CreditAmount > 0)
                    line.Account.Credit(line.CreditAmount);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Posted journal entry {EntryNumber}", entry.EntryNumber);

            return Ok(entry);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    #endregion

    #region Reports

    /// <summary>
    /// Get account balance for a specific account
    /// </summary>
    [HttpGet("accounts/{id}/balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetAccountBalance(int id)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (account == null)
            return NotFound();

        return Ok(new
        {
            accountId = account.Id,
            accountCode = account.AccountCode,
            accountName = account.Name,
            balance = account.CurrentBalance,
            currency = account.Currency
        });
    }

    /// <summary>
    /// Get balance sheet
    /// </summary>
    [HttpGet("reports/balance-sheet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetBalanceSheet([FromQuery] DateTime? asOfDate = null)
    {
        var targetDate = asOfDate ?? DateTime.Today;

        var assets = await _context.Accounts
            .Where(a => a.Type == AccountType.Asset && a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        var liabilities = await _context.Accounts
            .Where(a => a.Type == AccountType.Liability && a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        var equity = await _context.Accounts
            .Where(a => a.Type == AccountType.Equity && a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        return Ok(new
        {
            asOfDate = targetDate,
            assets = new
            {
                accounts = assets,
                total = assets.Sum(a => a.CurrentBalance)
            },
            liabilities = new
            {
                accounts = liabilities,
                total = liabilities.Sum(a => a.CurrentBalance)
            },
            equity = new
            {
                accounts = equity,
                total = equity.Sum(a => a.CurrentBalance)
            },
            balanceCheck = assets.Sum(a => a.CurrentBalance) == 
                           (liabilities.Sum(a => a.CurrentBalance) + equity.Sum(a => a.CurrentBalance))
        });
    }

    /// <summary>
    /// Get income statement (Profit & Loss)
    /// </summary>
    [HttpGet("reports/income-statement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetIncomeStatement(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today.AddMonths(-1);
        var end = endDate ?? DateTime.Today;

        var revenue = await _context.Accounts
            .Where(a => a.Type == AccountType.Revenue && a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        var expenses = await _context.Accounts
            .Where(a => a.Type == AccountType.Expense && a.IsActive)
            .OrderBy(a => a.AccountCode)
            .ToListAsync();

        var totalRevenue = revenue.Sum(a => a.CurrentBalance);
        var totalExpenses = expenses.Sum(a => a.CurrentBalance);

        return Ok(new
        {
            period = new { startDate = start, endDate = end },
            revenue = new
            {
                accounts = revenue,
                total = totalRevenue
            },
            expenses = new
            {
                accounts = expenses,
                total = totalExpenses
            },
            netIncome = totalRevenue - totalExpenses
        });
    }

    #endregion
}

// Request DTOs
public record CreateAccountRequest(
    string AccountCode,
    string Name,
    AccountType Type,
    string? Description,
    int? ParentAccountId,
    string? Currency
);

public record CreateJournalEntryRequest(
    DateTime EntryDate,
    string? ReferenceType,
    int? ReferenceId,
    string? ReferenceNumber,
    string? Description,
    string? Notes,
    List<CreateJournalEntryLineRequest> Lines
);

public record CreateJournalEntryLineRequest(
    int AccountId,
    decimal DebitAmount,
    decimal CreditAmount,
    string? Description
);

