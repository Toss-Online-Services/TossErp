using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Accounts.Application.Commands.CreateTransaction;
using Accounts.Application.Commands.UpdateTransaction;
using Accounts.Application.Commands.DeleteTransaction;
using Accounts.Application.Commands.ReverseTransaction;
using Accounts.Application.Commands.PostTransaction;
using Accounts.Application.Commands.BatchCreateTransactions;
using Accounts.Application.Queries.GetTransaction;
using Accounts.Application.Queries.GetTransactions;
using Accounts.Application.Queries.GetTransactionsByAccount;
using Accounts.Application.Queries.GetTransactionsByDateRange;
using Accounts.Application.Queries.GetTransactionsByReference;
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
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all transactions with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? accountId = null,
        [FromQuery] TransactionStatus? status = null,
        [FromQuery] string? reference = null,
        [FromQuery] string? description = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTransactionsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                AccountId = accountId,
                Status = status,
                Reference = reference,
                Description = description,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            
            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, "An error occurred while retrieving transactions");
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
    {
        try
        {
            var query = new GetTransactionQuery { Id = id };
            var transaction = await _mediator.Send(query);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
            return StatusCode(500, "An error occurred while retrieving the transaction");
        }
    }

    /// <summary>
    /// Get transactions by account
    /// </summary>
    [HttpGet("by-account/{accountId}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByAccount(
        Guid accountId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] TransactionStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTransactionsByAccountQuery
            {
                AccountId = accountId,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for account {AccountId}", accountId);
            return StatusCode(500, "An error occurred while retrieving transactions by account");
        }
    }

    /// <summary>
    /// Get transactions by date range
    /// </summary>
    [HttpGet("by-date-range")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByDateRange(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] TransactionStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTransactionsByDateRangeQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions by date range");
            return StatusCode(500, "An error occurred while retrieving transactions by date range");
        }
    }

    /// <summary>
    /// Get transactions by reference
    /// </summary>
    [HttpGet("by-reference/{reference}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByReference(
        string reference,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetTransactionsByReferenceQuery
            {
                Reference = reference,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            // Add pagination headers
            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)result.TotalCount / pageSize)).ToString());

            return Ok(result.Transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions by reference {Reference}", reference);
            return StatusCode(500, "An error occurred while retrieving transactions by reference");
        }
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = await _mediator.Send(command);
            
            _logger.LogInformation("Transaction created: {TransactionId} - {Reference}", 
                transaction.Id, transaction.Reference);
            
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid transaction creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, "An error occurred while creating the transaction");
        }
    }

    /// <summary>
    /// Create multiple transactions in batch
    /// </summary>
    [HttpPost("batch")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> BatchCreateTransactions([FromBody] BatchCreateTransactionsCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transactions = await _mediator.Send(command);
            
            _logger.LogInformation("Batch created {TransactionCount} transactions", transactions.Count());
            
            return Ok(transactions);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid batch transaction creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch transactions");
            return StatusCode(500, "An error occurred while creating batch transactions");
        }
    }

    /// <summary>
    /// Update an existing transaction
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<TransactionDto>> UpdateTransaction(Guid id, [FromBody] UpdateTransactionCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Transaction ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transaction = await _mediator.Send(command);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            _logger.LogInformation("Transaction updated: {TransactionId} - {Reference}", 
                transaction.Id, transaction.Reference);

            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid transaction update attempt for {TransactionId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction {TransactionId}", id);
            return StatusCode(500, "An error occurred while updating the transaction");
        }
    }

    /// <summary>
    /// Delete a transaction (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            var command = new DeleteTransactionCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            _logger.LogInformation("Transaction deleted: {TransactionId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete transaction {TransactionId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            return StatusCode(500, "An error occurred while deleting the transaction");
        }
    }

    /// <summary>
    /// Post a transaction (mark as posted/finalized)
    /// </summary>
    [HttpPost("{id}/post")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> PostTransaction(Guid id)
    {
        try
        {
            var command = new PostTransactionCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            _logger.LogInformation("Transaction posted: {TransactionId}", id);

            return Ok(new { message = "Transaction posted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot post transaction {TransactionId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting transaction {TransactionId}", id);
            return StatusCode(500, "An error occurred while posting the transaction");
        }
    }

    /// <summary>
    /// Reverse a transaction
    /// </summary>
    [HttpPost("{id}/reverse")]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<ActionResult<TransactionDto>> ReverseTransaction(Guid id, [FromBody] ReverseTransactionCommand command)
    {
        try
        {
            if (id != command.TransactionId)
            {
                return BadRequest("Transaction ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reversalTransaction = await _mediator.Send(command);

            if (reversalTransaction == null)
            {
                return NotFound($"Transaction with ID {id} not found");
            }

            _logger.LogInformation("Transaction reversed: {OriginalTransactionId} -> {ReversalTransactionId}", 
                id, reversalTransaction.Id);

            return Ok(reversalTransaction);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reverse transaction {TransactionId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing transaction {TransactionId}", id);
            return StatusCode(500, "An error occurred while reversing the transaction");
        }
    }

    /// <summary>
    /// Export transactions to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> ExportTransactionsToExcel(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? accountId = null,
        [FromQuery] TransactionStatus? status = null,
        [FromQuery] string? reference = null)
    {
        try
        {
            var query = new GetTransactionsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                AccountId = accountId,
                Status = status,
                Reference = reference,
                Page = 1,
                PageSize = int.MaxValue // Get all transactions for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transactions");

            // Headers
            worksheet.Cell(1, 1).Value = "Transaction Date";
            worksheet.Cell(1, 2).Value = "Reference";
            worksheet.Cell(1, 3).Value = "Description";
            worksheet.Cell(1, 4).Value = "Account";
            worksheet.Cell(1, 5).Value = "Debit Amount";
            worksheet.Cell(1, 6).Value = "Credit Amount";
            worksheet.Cell(1, 7).Value = "Status";
            worksheet.Cell(1, 8).Value = "Created Date";
            worksheet.Cell(1, 9).Value = "Posted Date";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var transaction in result.Transactions)
            {
                worksheet.Cell(row, 1).Value = transaction.TransactionDate;
                worksheet.Cell(row, 2).Value = transaction.Reference;
                worksheet.Cell(row, 3).Value = transaction.Description;
                worksheet.Cell(row, 4).Value = transaction.AccountName;
                worksheet.Cell(row, 5).Value = transaction.DebitAmount;
                worksheet.Cell(row, 6).Value = transaction.CreditAmount;
                worksheet.Cell(row, 7).Value = transaction.Status.ToString();
                worksheet.Cell(row, 8).Value = transaction.CreatedAt;
                worksheet.Cell(row, 9).Value = transaction.PostedAt;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Transactions_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {TransactionCount} transactions to Excel", result.Transactions.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting transactions to Excel");
            return StatusCode(500, "An error occurred while exporting transactions");
        }
    }

    /// <summary>
    /// Get transaction statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<object>> GetTransactionStatistics(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetTransactionsQuery
            {
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTime.UtcNow,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalTransactions = result.TotalCount,
                PendingTransactions = result.Transactions.Count(t => t.Status == TransactionStatus.Pending),
                PostedTransactions = result.Transactions.Count(t => t.Status == TransactionStatus.Posted),
                ReversedTransactions = result.Transactions.Count(t => t.Status == TransactionStatus.Reversed),
                TotalDebitAmount = result.Transactions.Sum(t => t.DebitAmount),
                TotalCreditAmount = result.Transactions.Sum(t => t.CreditAmount),
                AverageTransactionAmount = result.Transactions.Any() ? 
                    result.Transactions.Average(t => Math.Max(t.DebitAmount, t.CreditAmount)) : 0,
                TransactionsByDay = result.Transactions
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count(), Amount = g.Sum(t => Math.Max(t.DebitAmount, t.CreditAmount)) })
                    .OrderBy(x => x.Date)
                    .ToList(),
                LargestTransaction = result.Transactions.Any() ?
                    result.Transactions.Max(t => Math.Max(t.DebitAmount, t.CreditAmount)) : 0,
                SmallestTransaction = result.Transactions.Any() ?
                    result.Transactions.Min(t => Math.Max(t.DebitAmount, t.CreditAmount)) : 0
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction statistics");
            return StatusCode(500, "An error occurred while retrieving transaction statistics");
        }
    }

    /// <summary>
    /// Get daily transaction summary
    /// </summary>
    [HttpGet("daily-summary")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<object>> GetDailyTransactionSummary(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetTransactionsQuery
            {
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTime.UtcNow,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var dailySummary = result.Transactions
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TransactionCount = g.Count(),
                    TotalDebitAmount = g.Sum(t => t.DebitAmount),
                    TotalCreditAmount = g.Sum(t => t.CreditAmount),
                    NetAmount = g.Sum(t => t.CreditAmount - t.DebitAmount),
                    PendingCount = g.Count(t => t.Status == TransactionStatus.Pending),
                    PostedCount = g.Count(t => t.Status == TransactionStatus.Posted)
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(dailySummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving daily transaction summary");
            return StatusCode(500, "An error occurred while retrieving daily transaction summary");
        }
    }
}
