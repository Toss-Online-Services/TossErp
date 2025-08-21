using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Accounts.Application.Commands.CreateJournalEntry;
using Accounts.Application.Commands.UpdateJournalEntry;
using Accounts.Application.Commands.DeleteJournalEntry;
using Accounts.Application.Commands.PostJournalEntry;
using Accounts.Application.Commands.ReverseJournalEntry;
using Accounts.Application.Queries.GetJournalEntry;
using Accounts.Application.Queries.GetJournalEntries;
using Accounts.Application.Queries.GetJournalEntriesByPeriod;
using Accounts.Application.Queries.GetJournalEntriesByReference;
using Accounts.Application.DTOs;
using Accounts.Domain.Enums;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Accounts.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class JournalEntriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<JournalEntriesController> _logger;

    public JournalEntriesController(IMediator mediator, ILogger<JournalEntriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all journal entries with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetJournalEntries(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] JournalEntryStatus? status = null,
        [FromQuery] string? reference = null,
        [FromQuery] string? description = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetJournalEntriesQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
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

            return Ok(result.JournalEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entries");
            return StatusCode(500, "An error occurred while retrieving journal entries");
        }
    }

    /// <summary>
    /// Get journal entry by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntryDto>> GetJournalEntry(Guid id)
    {
        try
        {
            var query = new GetJournalEntryQuery { Id = id };
            var journalEntry = await _mediator.Send(query);

            if (journalEntry == null)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            return Ok(journalEntry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entry {JournalEntryId}", id);
            return StatusCode(500, "An error occurred while retrieving the journal entry");
        }
    }

    /// <summary>
    /// Get journal entries by period
    /// </summary>
    [HttpGet("by-period")]
    public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetJournalEntriesByPeriod(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] JournalEntryStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetJournalEntriesByPeriodQuery
            {
                Year = year,
                Month = month,
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

            return Ok(result.JournalEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entries for period {Year}-{Month}", year, month);
            return StatusCode(500, "An error occurred while retrieving journal entries by period");
        }
    }

    /// <summary>
    /// Get journal entries by reference
    /// </summary>
    [HttpGet("by-reference/{reference}")]
    public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetJournalEntriesByReference(
        string reference,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = new GetJournalEntriesByReferenceQuery
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

            return Ok(result.JournalEntries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entries by reference {Reference}", reference);
            return StatusCode(500, "An error occurred while retrieving journal entries by reference");
        }
    }

    /// <summary>
    /// Create a new journal entry
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry([FromBody] CreateJournalEntryCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var journalEntry = await _mediator.Send(command);
            
            _logger.LogInformation("Journal entry created: {JournalEntryId} - {Reference}", 
                journalEntry.Id, journalEntry.Reference);
            
            return CreatedAtAction(nameof(GetJournalEntry), new { id = journalEntry.Id }, journalEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid journal entry creation attempt");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating journal entry");
            return StatusCode(500, "An error occurred while creating the journal entry");
        }
    }

    /// <summary>
    /// Update an existing journal entry
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<JournalEntryDto>> UpdateJournalEntry(Guid id, [FromBody] UpdateJournalEntryCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest("Journal entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var journalEntry = await _mediator.Send(command);

            if (journalEntry == null)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            _logger.LogInformation("Journal entry updated: {JournalEntryId} - {Reference}", 
                journalEntry.Id, journalEntry.Reference);

            return Ok(journalEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid journal entry update attempt for {JournalEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating journal entry {JournalEntryId}", id);
            return StatusCode(500, "An error occurred while updating the journal entry");
        }
    }

    /// <summary>
    /// Delete a journal entry (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteJournalEntry(Guid id)
    {
        try
        {
            var command = new DeleteJournalEntryCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            _logger.LogInformation("Journal entry deleted: {JournalEntryId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete journal entry {JournalEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting journal entry {JournalEntryId}", id);
            return StatusCode(500, "An error occurred while deleting the journal entry");
        }
    }

    /// <summary>
    /// Post a journal entry (mark as posted/finalized)
    /// </summary>
    [HttpPost("{id}/post")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> PostJournalEntry(Guid id)
    {
        try
        {
            var command = new PostJournalEntryCommand { Id = id };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            _logger.LogInformation("Journal entry posted: {JournalEntryId}", id);

            return Ok(new { message = "Journal entry posted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot post journal entry {JournalEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting journal entry {JournalEntryId}", id);
            return StatusCode(500, "An error occurred while posting the journal entry");
        }
    }

    /// <summary>
    /// Reverse a journal entry
    /// </summary>
    [HttpPost("{id}/reverse")]
    [Authorize(Roles = "Admin,AccountManager")]
    public async Task<ActionResult<JournalEntryDto>> ReverseJournalEntry(Guid id, [FromBody] ReverseJournalEntryCommand command)
    {
        try
        {
            if (id != command.JournalEntryId)
            {
                return BadRequest("Journal entry ID in URL does not match the ID in the request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reversalEntry = await _mediator.Send(command);

            if (reversalEntry == null)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            _logger.LogInformation("Journal entry reversed: {OriginalEntryId} -> {ReversalEntryId}", 
                id, reversalEntry.Id);

            return Ok(reversalEntry);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot reverse journal entry {JournalEntryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing journal entry {JournalEntryId}", id);
            return StatusCode(500, "An error occurred while reversing the journal entry");
        }
    }

    /// <summary>
    /// Export journal entries to Excel
    /// </summary>
    [HttpGet("export/excel")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> ExportJournalEntriesToExcel(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] JournalEntryStatus? status = null,
        [FromQuery] string? reference = null)
    {
        try
        {
            var query = new GetJournalEntriesQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                Reference = reference,
                Page = 1,
                PageSize = int.MaxValue // Get all entries for export
            };

            var result = await _mediator.Send(query);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Journal Entries");

            // Headers
            worksheet.Cell(1, 1).Value = "Entry Date";
            worksheet.Cell(1, 2).Value = "Reference";
            worksheet.Cell(1, 3).Value = "Description";
            worksheet.Cell(1, 4).Value = "Total Amount";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Created Date";
            worksheet.Cell(1, 7).Value = "Posted Date";
            worksheet.Cell(1, 8).Value = "Created By";

            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data
            int row = 2;
            foreach (var entry in result.JournalEntries)
            {
                worksheet.Cell(row, 1).Value = entry.EntryDate;
                worksheet.Cell(row, 2).Value = entry.Reference;
                worksheet.Cell(row, 3).Value = entry.Description;
                worksheet.Cell(row, 4).Value = entry.TotalAmount;
                worksheet.Cell(row, 5).Value = entry.Status.ToString();
                worksheet.Cell(row, 6).Value = entry.CreatedAt;
                worksheet.Cell(row, 7).Value = entry.PostedAt;
                worksheet.Cell(row, 8).Value = entry.CreatedBy;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"JournalEntries_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            _logger.LogInformation("Exported {EntryCount} journal entries to Excel", result.JournalEntries.Count());

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting journal entries to Excel");
            return StatusCode(500, "An error occurred while exporting journal entries");
        }
    }

    /// <summary>
    /// Export journal entry to PDF
    /// </summary>
    [HttpGet("{id}/export/pdf")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<IActionResult> ExportJournalEntryToPdf(Guid id)
    {
        try
        {
            var query = new GetJournalEntryQuery { Id = id };
            var journalEntry = await _mediator.Send(query);

            if (journalEntry == null)
            {
                return NotFound($"Journal entry with ID {id} not found");
            }

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var title = new Paragraph("Journal Entry", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Entry info
            var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            document.Add(new Paragraph($"Reference: {journalEntry.Reference}", infoFont));
            document.Add(new Paragraph($"Entry Date: {journalEntry.EntryDate:yyyy-MM-dd}", infoFont));
            document.Add(new Paragraph($"Description: {journalEntry.Description}", infoFont));
            document.Add(new Paragraph($"Total Amount: {journalEntry.TotalAmount:C}", infoFont));
            document.Add(new Paragraph($"Status: {journalEntry.Status}", infoFont));
            document.Add(new Paragraph($"Created: {journalEntry.CreatedAt:yyyy-MM-dd HH:mm} UTC by {journalEntry.CreatedBy}", infoFont));
            if (journalEntry.PostedAt.HasValue)
            {
                document.Add(new Paragraph($"Posted: {journalEntry.PostedAt:yyyy-MM-dd HH:mm} UTC", infoFont));
            }
            document.Add(new Paragraph(" ")); // Empty line

            // Line items table
            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 40, 20, 20, 20 });

            // Headers
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("Account", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Description", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("Debit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase("Credit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });

            // Data
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            foreach (var lineItem in journalEntry.LineItems)
            {
                table.AddCell(new PdfPCell(new Phrase(lineItem.AccountName, cellFont)));
                table.AddCell(new PdfPCell(new Phrase(lineItem.Description ?? "", cellFont)));
                table.AddCell(new PdfPCell(new Phrase(lineItem.DebitAmount > 0 ? lineItem.DebitAmount.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(lineItem.CreditAmount > 0 ? lineItem.CreditAmount.ToString("C") : "", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            // Totals
            var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("TOTAL", totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, Colspan = 2 });
            table.AddCell(new PdfPCell(new Phrase(journalEntry.LineItems.Sum(l => l.DebitAmount).ToString("C"), totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });
            table.AddCell(new PdfPCell(new Phrase(journalEntry.LineItems.Sum(l => l.CreditAmount).ToString("C"), totalFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT });

            document.Add(table);
            document.Close();

            var fileName = $"Journal_Entry_{journalEntry.Reference}_{journalEntry.EntryDate:yyyyMMdd}.pdf";
            
            _logger.LogInformation("Exported journal entry {JournalEntryId} to PDF", id);

            return File(stream.ToArray(), "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting journal entry {JournalEntryId} to PDF", id);
            return StatusCode(500, "An error occurred while exporting the journal entry");
        }
    }

    /// <summary>
    /// Get journal entry statistics
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<object>> GetJournalEntryStatistics(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetJournalEntriesQuery
            {
                FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
                ToDate = toDate ?? DateTime.UtcNow,
                Page = 1,
                PageSize = int.MaxValue
            };

            var result = await _mediator.Send(query);

            var stats = new
            {
                TotalEntries = result.TotalCount,
                DraftEntries = result.JournalEntries.Count(j => j.Status == JournalEntryStatus.Draft),
                PostedEntries = result.JournalEntries.Count(j => j.Status == JournalEntryStatus.Posted),
                ReversedEntries = result.JournalEntries.Count(j => j.Status == JournalEntryStatus.Reversed),
                TotalAmount = result.JournalEntries.Sum(j => j.TotalAmount),
                AverageEntryAmount = result.JournalEntries.Any() ? 
                    result.JournalEntries.Average(j => j.TotalAmount) : 0,
                EntriesByDay = result.JournalEntries
                    .GroupBy(j => j.EntryDate.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count(), Amount = g.Sum(j => j.TotalAmount) })
                    .OrderBy(x => x.Date)
                    .ToList(),
                LargestEntry = result.JournalEntries.Any() ?
                    result.JournalEntries.Max(j => j.TotalAmount) : 0,
                SmallestEntry = result.JournalEntries.Any() ?
                    result.JournalEntries.Min(j => j.TotalAmount) : 0
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journal entry statistics");
            return StatusCode(500, "An error occurred while retrieving journal entry statistics");
        }
    }

    /// <summary>
    /// Validate journal entry balancing
    /// </summary>
    [HttpPost("validate")]
    [Authorize(Roles = "Admin,AccountManager,Accountant")]
    public async Task<ActionResult<object>> ValidateJournalEntry([FromBody] CreateJournalEntryCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var totalDebits = command.LineItems.Sum(l => l.DebitAmount);
            var totalCredits = command.LineItems.Sum(l => l.CreditAmount);
            var isBalanced = totalDebits == totalCredits;

            var validation = new
            {
                IsBalanced = isBalanced,
                TotalDebits = totalDebits,
                TotalCredits = totalCredits,
                Difference = totalDebits - totalCredits,
                LineItemCount = command.LineItems.Count,
                Errors = new List<string>()
            };

            var errors = (List<string>)validation.Errors;

            if (!isBalanced)
            {
                errors.Add($"Journal entry is not balanced. Difference: {validation.Difference:C}");
            }

            if (command.LineItems.Count < 2)
            {
                errors.Add("Journal entry must have at least 2 line items");
            }

            if (command.LineItems.Any(l => l.DebitAmount == 0 && l.CreditAmount == 0))
            {
                errors.Add("All line items must have either a debit or credit amount");
            }

            if (command.LineItems.Any(l => l.DebitAmount > 0 && l.CreditAmount > 0))
            {
                errors.Add("Line items cannot have both debit and credit amounts");
            }

            return Ok(validation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating journal entry");
            return StatusCode(500, "An error occurred while validating the journal entry");
        }
    }
}
