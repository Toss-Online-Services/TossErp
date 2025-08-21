using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Accounts.Application.Commands.CreateAccount;
using Accounts.Application.Commands.UpdateAccount;
using Accounts.Application.Commands.DeleteAccount;
using Accounts.Application.Commands.ActivateAccount;
using Accounts.Application.Commands.DeactivateAccount;
using Accounts.Application.Commands.ReconcileAccount;
using Accounts.Application.Queries.GetAccount;
using Accounts.Application.Queries.GetAccounts;
using Accounts.Application.Queries.GetAccountsByType;
using Accounts.Application.Queries.GetAccountBalance;
using Accounts.Application.Queries.GetAccountStatement;
using Accounts.Application.Queries.GetAccountReconciliation;
using Accounts.Application.DTOs;
using Accounts.Domain.Enums;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace Accounts.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all accounts with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts(
        [FromQuery] AccountType? accountType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetAccountsQuery
            {
                AccountType = accountType,
                IsActive = isActive,
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts");
            return StatusCode(500, "An error occurred while retrieving accounts");
        }
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(Guid id)
    {
        try
        {
            var query = new GetAccountQuery { Id = id };
            var account = await _mediator.Send(query);

            if (account == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving the account");
        }
    }

    /// <summary>
    /// Get accounts by type
    /// </summary>
    [HttpGet("by-type/{accountType}")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByType(
        AccountType accountType,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetAccountsByTypeQuery
            {
                AccountType = accountType,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts by type {AccountType}", accountType);
            return StatusCode(500, "An error occurred while retrieving accounts by type");
        }
    }

    /// <summary>
    /// Get account balance
    /// </summary>
    [HttpGet("{id}/balance")]
    public async Task<ActionResult<AccountBalanceDto>> GetAccountBalance(Guid id, [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            var query = new GetAccountBalanceQuery 
            { 
                AccountId = id,
                AsOfDate = asOfDate ?? DateTime.UtcNow
            };

            var balance = await _mediator.Send(query);

            if (balance == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            return Ok(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving account balance");
        }
    }

    /// <summary>
    /// Get account statement
    /// </summary>
    [HttpGet("{id}/statement")]
    public async Task<ActionResult<AccountStatementDto>> GetAccountStatement(
        Guid id, 
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var query = new GetAccountStatementQuery
            {
                AccountId = id,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var statement = await _mediator.Send(query);

            if (statement == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            return Ok(statement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statement for account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving account statement");
        }
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _mediator.Send(command);
            
            _logger.LogInformation("Account created: {AccountId} - {AccountName}", account.Id, account.Name);
            
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, "An error occurred while creating the account");
        }
    }

    /// <summary>
    /// Update an existing account
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<ActionResult<AccountDto>> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Account ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _mediator.Send(command);

            if (account == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            _logger.LogInformation("Account updated: {AccountId} - {AccountName}", account.Id, account.Name);

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId}", id);
            return StatusCode(500, "An error occurred while updating the account");
        }
    }

    /// <summary>
    /// Delete an account (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        try
        {
            var command = new DeleteAccountCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Account with ID {id} not found");
            }

            _logger.LogInformation("Account deleted: {AccountId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete account {AccountId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account {AccountId}", id);
            return StatusCode(500, "An error occurred while deleting the account");
        }
    }

    /// <summary>
    /// Activate an account
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<IActionResult> ActivateAccount(Guid id)
    {
        try
        {
            var command = new ActivateAccountCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Account with ID {id} not found");
            }

            _logger.LogInformation("Account activated: {AccountId}", id);

            return Ok(new { message = "Account activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating account {AccountId}", id);
            return StatusCode(500, "An error occurred while activating the account");
        }
    }

    /// <summary>
    /// Deactivate an account
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<IActionResult> DeactivateAccount(Guid id)
    {
        try
        {
            var command = new DeactivateAccountCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Account with ID {id} not found");
            }

            _logger.LogInformation("Account deactivated: {AccountId}", id);

            return Ok(new { message = "Account deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating account {AccountId}", id);
            return StatusCode(500, "An error occurred while deactivating the account");
        }
    }

    /// <summary>
    /// Reconcile an account
    /// </summary>
    [HttpPost("{id}/reconcile")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<AccountReconciliationDto>> ReconcileAccount(Guid id, [FromBody] ReconcileAccountCommand command)
    {
        try
        {
            if (id != command.AccountId)
            {
                return BadRequest("Account ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reconciliation = await _mediator.Send(command);

            if (reconciliation == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            _logger.LogInformation("Account reconciled: {AccountId} - Date: {ReconciliationDate}", id, command.ReconciliationDate);

            return Ok(reconciliation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reconciling account {AccountId}", id);
            return StatusCode(500, "An error occurred while reconciling the account");
        }
    }

    /// <summary>
    /// Get account reconciliation history
    /// </summary>
    [HttpGet("{id}/reconciliations")]
    public async Task<ActionResult<IEnumerable<AccountReconciliationDto>>> GetAccountReconciliations(
        Guid id,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetAccountReconciliationQuery
            {
                AccountId = id,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Reconciliations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reconciliations for account {AccountId}", id);
            return StatusCode(500, "An error occurred while retrieving account reconciliations");
        }
    }

    /// <summary>
    /// Export accounts to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> ExportAccountsToExcel(
        [FromQuery] AccountType? accountType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var query = new GetAccountsQuery
            {
                AccountType = accountType,
                IsActive = isActive,
                SearchTerm = searchTerm,
                Page = 1,
                PageSize = int.MaxValue // Get all accounts for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Accounts");

            // Headers
            worksheet.Cell(1, 1).Value = "Account Code";
            worksheet.Cell(1, 2).Value = "Account Name";
            worksheet.Cell(1, 3).Value = "Account Type";
            worksheet.Cell(1, 4).Value = "Parent Account";
            worksheet.Cell(1, 5).Value = "Current Balance";
            worksheet.Cell(1, 6).Value = "Is Active";
            worksheet.Cell(1, 7).Value = "Created Date";
            worksheet.Cell(1, 8).Value = "Description";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var account in result.Accounts)
            {
                worksheet.Cell(row, 1).Value = account.Code;
                worksheet.Cell(row, 2).Value = account.Name;
                worksheet.Cell(row, 3).Value = account.AccountType.ToString();
                worksheet.Cell(row, 4).Value = account.ParentAccountName ?? "";
                worksheet.Cell(row, 5).Value = account.CurrentBalance;
                worksheet.Cell(row, 6).Value = account.IsActive ? "Yes" : "No";
                worksheet.Cell(row, 7).Value = account.CreatedAt;
                worksheet.Cell(row, 8).Value = account.Description ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Accounts_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {AccountCount} accounts to Excel", result.Accounts.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting accounts to Excel");
            return StatusCode(500, "An error occurred while exporting accounts");
        }
    }

    /// <summary>
    /// Export account statement to PDF
    /// </summary>
    [HttpGet("{id}/statement/export/pdf")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> ExportStatementToPdf(
        Guid id, 
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate)
    {
        try
        {
            var query = new GetAccountStatementQuery
            {
                AccountId = id,
                FromDate = fromDate,
                ToDate = toDate,
                Page = 1,
                PageSize = int.MaxValue // Get all transactions for export
            };

            var statement = await _mediator.Send(query);

            if (statement == null)
            {
                return NotFound($"Account with ID {id} not found");
            }

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var title = new Paragraph("Account Statement", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Account info
            var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            document.Add(new Paragraph($"Account: {statement.AccountName} ({statement.AccountCode})", infoFont));
            document.Add(new Paragraph($"Statement Period: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}", infoFont));
            document.Add(new Paragraph($"Opening Balance: {statement.OpeningBalance:C}", infoFont));
            document.Add(new Paragraph($"Closing Balance: {statement.ClosingBalance:C}", infoFont));
            document.Add(new Paragraph($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC", infoFont));
            document.Add(new Paragraph(" ")); // Empty line

            // Transactions table
            var table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 15, 40, 15, 15, 15 });

            // Headers
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("Date", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Description", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Debit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase("Credit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase("Balance", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });

            // Data
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            foreach (var transaction in statement.Transactions)
            {
                table.AddCell(new PdfPCell(new Phrase(transaction.TransactionDate.ToString("yyyy-MM-dd"), cellFont)));
                table.AddCell(new PdfPCell(new Phrase(transaction.Description, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(transaction.DebitAmount > 0 ? transaction.DebitAmount.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(transaction.CreditAmount > 0 ? transaction.CreditAmount.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(transaction.RunningBalance.ToString("C"), cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            document.Add(table);
            document.Close();

            var fileName = $"Account_Statement_{statement.AccountCode}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf";
            
            _logger.LogInformation("Exported statement for account {AccountId} to PDF", id);

            return File(stream.ToArray(), "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting statement to PDF for account {AccountId}", id);
            return StatusCode(500, "An error occurred while exporting the statement");
        }
    }

    /// <summary>
    /// Get account statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<object>> GetAccountStatistics()
    {
        try
        {
            var query = new GetAccountsQuery
            {
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalAccounts = result.TotalCount,
                ActiveAccounts = result.Accounts.Count(a => a.IsActive),
                InactiveAccounts = result.Accounts.Count(a => !a.IsActive),
                AccountsByType = result.Accounts.GroupBy(a => a.AccountType)
                    .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                TotalBalance = result.Accounts.Sum(a => a.CurrentBalance),
                AssetAccounts = result.Accounts.Count(a => a.AccountType == AccountType.Asset),
                LiabilityAccounts = result.Accounts.Count(a => a.AccountType == AccountType.Liability),
                EquityAccounts = result.Accounts.Count(a => a.AccountType == AccountType.Equity),
                RevenueAccounts = result.Accounts.Count(a => a.AccountType == AccountType.Revenue),
                ExpenseAccounts = result.Accounts.Count(a => a.AccountType == AccountType.Expense)
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account statistics");
            return StatusCode(500, "An error occurred while retrieving account statistics");
        }
    }
}
