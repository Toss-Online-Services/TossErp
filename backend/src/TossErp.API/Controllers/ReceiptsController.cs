using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TossErp.Infrastructure.Services;

namespace TossErp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly ILogger<ReceiptsController> _logger;

    public ReceiptsController(IReceiptService receiptService, ILogger<ReceiptsController> logger)
    {
        _receiptService = receiptService;
        _logger = logger;
    }

    /// <summary>
    /// Generate receipt HTML for a sale
    /// </summary>
    [HttpGet("{saleId}/html")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceiptHtml(int saleId)
    {
        try
        {
            var html = await _receiptService.GenerateReceiptHtml(saleId);
            return Content(html, "text/html");
        }
        catch (ArgumentException)
        {
            return NotFound(new { error = $"Sale {saleId} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating receipt HTML for sale {SaleId}", saleId);
            return StatusCode(500, new { error = "Failed to generate receipt" });
        }
    }

    /// <summary>
    /// Generate receipt PDF for a sale
    /// </summary>
    [HttpGet("{saleId}/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceiptPdf(int saleId)
    {
        try
        {
            var pdfBytes = await _receiptService.GenerateReceiptPdf(saleId);
            return File(pdfBytes, "application/pdf", $"receipt-{saleId}.pdf");
        }
        catch (ArgumentException)
        {
            return NotFound(new { error = $"Sale {saleId} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating receipt PDF for sale {SaleId}", saleId);
            return StatusCode(500, new { error = "Failed to generate receipt PDF" });
        }
    }

    /// <summary>
    /// Email receipt to customer
    /// </summary>
    [HttpPost("{saleId}/email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmailReceipt(int saleId, [FromBody] EmailReceiptRequest request)
    {
        try
        {
            var success = await _receiptService.EmailReceipt(saleId, request.Email);
            
            if (success)
                return Ok(new { message = $"Receipt emailed to {request.Email}" });
            
            return BadRequest(new { error = "Failed to send email" });
        }
        catch (ArgumentException)
        {
            return NotFound(new { error = $"Sale {saleId} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error emailing receipt for sale {SaleId}", saleId);
            return StatusCode(500, new { error = "Failed to email receipt" });
        }
    }
}

public record EmailReceiptRequest(string Email);

