using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TossErp.Domain.Entities.Sales;
using TossErp.Infrastructure.Data;

namespace TossErp.Infrastructure.Services;

public interface IReceiptService
{
    Task<string> GenerateReceiptHtml(int saleId);
    Task<byte[]> GenerateReceiptPdf(int saleId);
    Task<bool> EmailReceipt(int saleId, string email);
}

public class ReceiptService : IReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReceiptService> _logger;

    public ReceiptService(ApplicationDbContext context, ILogger<ReceiptService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GenerateReceiptHtml(int saleId)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new ArgumentException($"Sale {saleId} not found");

        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Receipt - {sale.SaleNumber}</title>
    <style>
        body {{
            font-family: 'Courier New', monospace;
            max-width: 300px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            text-align: center;
            margin-bottom: 20px;
            border-bottom: 2px dashed #000;
            padding-bottom: 10px;
        }}
        .company-name {{
            font-size: 18px;
            font-weight: bold;
        }}
        .receipt-info {{
            margin: 15px 0;
            font-size: 12px;
        }}
        .items {{
            border-top: 1px dashed #000;
            border-bottom: 1px dashed #000;
            padding: 10px 0;
            margin: 10px 0;
        }}
        .item {{
            margin: 8px 0;
        }}
        .item-name {{
            font-weight: bold;
        }}
        .item-details {{
            display: flex;
            justify-content: space-between;
            font-size: 12px;
        }}
        .totals {{
            margin-top: 15px;
        }}
        .total-line {{
            display: flex;
            justify-content: space-between;
            margin: 5px 0;
        }}
        .total-line.grand-total {{
            font-weight: bold;
            font-size: 16px;
            border-top: 2px solid #000;
            padding-top: 10px;
            margin-top: 10px;
        }}
        .payments {{
            margin: 15px 0;
            border-top: 1px dashed #000;
            padding-top: 10px;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            font-size: 11px;
            border-top: 2px dashed #000;
            padding-top: 10px;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <div class=""company-name"">TOSS ERP</div>
        <div>Township One-Stop Solution</div>
    </div>

    <div class=""receipt-info"">
        <div><strong>Receipt #:</strong> {sale.ReceiptNumber ?? sale.SaleNumber}</div>
        <div><strong>Date:</strong> {sale.SaleDate:yyyy-MM-dd HH:mm}</div>
        {(sale.CustomerName != null ? $"<div><strong>Customer:</strong> {sale.CustomerName}</div>" : "")}
        {(sale.CashierName != null ? $"<div><strong>Cashier:</strong> {sale.CashierName}</div>" : "")}
        {(sale.WarehouseName != null ? $"<div><strong>Store:</strong> {sale.WarehouseName}</div>" : "")}
    </div>

    <div class=""items"">
        {string.Join("", sale.Items.Select(item => $@"
        <div class=""item"">
            <div class=""item-name"">{item.ProductName}</div>
            <div class=""item-details"">
                <span>{item.Quantity} x R{(item.UnitPrice / 100):F2}</span>
                <span>R{(item.LineTotal / 100):F2}</span>
            </div>
            {(item.Discount > 0 ? $@"<div class=""item-details"" style=""color: #090;""><span>Discount</span><span>-R{(item.Discount / 100):F2}</span></div>" : "")}
        </div>"))}
    </div>

    <div class=""totals"">
        <div class=""total-line"">
            <span>Subtotal:</span>
            <span>R{(sale.Subtotal / 100):F2}</span>
        </div>
        {(sale.DiscountAmount > 0 ? $@"
        <div class=""total-line"" style=""color: #090;"">
            <span>Discount:</span>
            <span>-R{(sale.DiscountAmount / 100):F2}</span>
        </div>" : "")}
        <div class=""total-line"">
            <span>Tax (15%):</span>
            <span>R{(sale.TaxAmount / 100):F2}</span>
        </div>
        <div class=""total-line grand-total"">
            <span>TOTAL:</span>
            <span>R{(sale.TotalAmount / 100):F2}</span>
        </div>
    </div>

    <div class=""payments"">
        <strong>Payments:</strong>
        {string.Join("", sale.Payments.Select(p => $@"
        <div class=""total-line"">
            <span>{p.Method}:</span>
            <span>R{(p.Amount / 100):F2}</span>
        </div>"))}
    </div>

    <div class=""footer"">
        <div>Thank you for your business!</div>
        <div style=""margin-top: 10px;"">
            For support: support@tosserp.com
        </div>
        <div style=""margin-top: 5px; font-size: 10px;"">
            Powered by TOSS ERP III
        </div>
    </div>
</body>
</html>";

        return html;
    }

    public async Task<byte[]> GenerateReceiptPdf(int saleId)
    {
        // For now, return HTML as bytes
        // In production, integrate a PDF library like:
        // - DinkToPdf (wrapper for wkhtmltopdf)
        // - iTextSharp / iText7
        // - PuppeteerSharp (headless Chrome)
        
        var html = await GenerateReceiptHtml(saleId);
        return System.Text.Encoding.UTF8.GetBytes(html);
        
        // TODO: Replace with actual PDF generation
        // Example with PuppeteerSharp:
        // await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        // await using var page = await browser.NewPageAsync();
        // await page.SetContentAsync(html);
        // return await page.PdfDataAsync();
    }

    public async Task<bool> EmailReceipt(int saleId, string email)
    {
        try
        {
            var html = await GenerateReceiptHtml(saleId);
            var sale = await _context.Sales.FindAsync(saleId);

            // TODO: Integrate with email service (SendGrid, SMTP, etc.)
            // For now, log the action
            _logger.LogInformation("Receipt for sale {SaleNumber} would be emailed to {Email}", 
                sale?.SaleNumber, email);

            // Production implementation:
            // await _emailService.SendAsync(
            //     to: email,
            //     subject: $"Receipt - {sale.SaleNumber}",
            //     htmlBody: html
            // );

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to email receipt for sale {SaleId}", saleId);
            return false;
        }
    }
}

