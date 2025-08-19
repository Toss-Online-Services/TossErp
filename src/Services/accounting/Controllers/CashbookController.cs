using Microsoft.AspNetCore.Mvc;
using MediatR;
using TossErp.Accounting.Application.Commands.CreateCashbook;
using TossErp.Accounting.Application.Commands.AddCashbookEntry;
using TossErp.Accounting.Application.Queries.GetCashbookEntries;
using TossErp.Accounting.Application.Queries.GetCashbookSummary;
using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Controllers;

/// <summary>
/// API controller for cashbook operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CashbookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CashbookController> _logger;

    public CashbookController(IMediator mediator, ILogger<CashbookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get cashbook entries with optional filtering
    /// </summary>
    [HttpGet("entries")]
    public async Task<ActionResult<CashbookEntriesResponse>> GetEntries(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? category = null,
        [FromQuery] string? type = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Getting cashbook entries with filters: From={FromDate}, To={ToDate}, Category={Category}, Type={Type}", 
                fromDate, toDate, category, type);

            var query = new GetCashbookEntriesQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Category = category,
                Type = type,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cashbook entries");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get cashbook summary
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<CashbookSummaryResponse>> GetSummary(
        [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            _logger.LogInformation("Getting cashbook summary as of {AsOfDate}", asOfDate);

            var query = new GetCashbookSummaryQuery
            {
                AsOfDate = asOfDate ?? DateTime.Today
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cashbook summary");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Add a new cashbook entry
    /// </summary>
    [HttpPost("entries")]
    public async Task<ActionResult<CashbookEntryDto>> AddEntry([FromBody] AddCashbookEntryRequest request)
    {
        try
        {
            _logger.LogInformation("Adding cashbook entry: Amount={Amount}, Type={Type}, Category={Category}", 
                request.Amount, request.Type, request.Category);

            var command = new AddCashbookEntryCommand
            {
                CashbookId = request.CashbookId,
                Amount = request.Amount,
                Type = request.Type,
                Category = request.Category,
                Description = request.Description,
                Reference = request.Reference,
                TransactionDate = request.TransactionDate
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEntries), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding cashbook entry");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Export cashbook entries to CSV
    /// </summary>
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? category = null,
        [FromQuery] string? type = null)
    {
        try
        {
            _logger.LogInformation("Exporting cashbook entries to CSV: From={FromDate}, To={ToDate}, Category={Category}, Type={Type}", 
                fromDate, toDate, category, type);

            var query = new GetCashbookEntriesQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Category = category,
                Type = type,
                Page = 1,
                PageSize = int.MaxValue // Get all entries for export
            };

            var result = await _mediator.Send(query);
            
            var csvContent = GenerateCsvContent(result.Entries);
            var fileName = $"cashbook_entries_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting cashbook entries to CSV");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get available filter options
    /// </summary>
    [HttpGet("filters")]
    public async Task<ActionResult<CashbookFiltersResponse>> GetFilters()
    {
        try
        {
            _logger.LogInformation("Getting cashbook filter options");

            // For MVP, return hardcoded filter options
            // In a real implementation, this would query the database for available values
            var filters = new CashbookFiltersResponse
            {
                Categories = new[] { "Sale", "Purchase", "CashReceipt", "CashPayment", "Adjustment", "SalesTax", "PurchaseTax" },
                Types = new[] { "Debit", "Credit" },
                DateRange = new DateRangeFilter
                {
                    MinDate = DateTime.Today.AddYears(-1),
                    MaxDate = DateTime.Today
                }
            };

            return Ok(filters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cashbook filters");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    private string GenerateCsvContent(IEnumerable<CashbookEntryDto> entries)
    {
        var csv = new System.Text.StringBuilder();
        
        // Header
        csv.AppendLine("Date,Type,Category,Amount,Description,Reference");
        
        // Data rows
        foreach (var entry in entries)
        {
            csv.AppendLine($"{entry.TransactionDate:yyyy-MM-dd}," +
                          $"{entry.Type}," +
                          $"{entry.Category}," +
                          $"{entry.Amount:F2}," +
                          $"\"{entry.Description?.Replace("\"", "\"\"")}\"," +
                          $"\"{entry.Reference?.Replace("\"", "\"\"")}\"");
        }
        
        return csv.ToString();
    }
}
