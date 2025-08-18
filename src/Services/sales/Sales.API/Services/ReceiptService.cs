using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Implementation of IReceiptService for generating receipts
/// </summary>
public class ReceiptService : IReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly ISaleRepository _saleRepository;
    private readonly ITillRepository _tillRepository;

    public ReceiptService(ILogger<ReceiptService> logger, ISaleRepository saleRepository, ITillRepository tillRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _tillRepository = tillRepository;
    }

    public async Task<string> GenerateReceiptAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating receipt for sale {SaleId}", sale.Id);

        var template = await GetReceiptTemplateAsync(cancellationToken);
        var receipt = template
            .Replace("{RECEIPT_NUMBER}", sale.ReceiptNumber.Value)
            .Replace("{DATE}", sale.CreatedAt.ToString("dd/MM/yyyy"))
            .Replace("{TIME}", sale.CreatedAt.ToString("HH:mm:ss"))
            .Replace("{TILL_NAME}", await GetTillNameAsync(sale.TillId, cancellationToken))
            .Replace("{CUSTOMER_NAME}", sale.CustomerName ?? "Walk-in Customer")
            .Replace("{ITEMS}", GenerateItemsSection(sale))
            .Replace("{SUBTOTAL}", sale.SubtotalAmount.ToString())
            .Replace("{TAX_AMOUNT}", sale.TaxAmount.ToString())
            .Replace("{DISCOUNT_AMOUNT}", sale.DiscountAmount > 0 ? sale.DiscountAmount.ToString() : "0.00")
            .Replace("{TOTAL}", sale.TotalAmount.ToString())
            .Replace("{PAYMENT_METHODS}", GeneratePaymentMethodsSection(sale))
            .Replace("{THANK_YOU}", "Thank you for your purchase!");

        return receipt;
    }

    public async Task<string> GenerateReceiptNumberAsync(Guid tillId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating receipt number for till {TillId}", tillId);

        // Get the last sale for this till to determine the next number
        var today = DateTime.Today;
        var sales = await _saleRepository.GetByDateRangeAsync(today, today.AddDays(1), cancellationToken);
        var tillSales = sales.Where(s => s.TillId == tillId).ToList();

        var nextNumber = tillSales.Count + 1;
        var receiptNumber = $"{today:yyyyMMdd}-{tillId.ToString().Substring(0, 4)}-{nextNumber:D4}";

        _logger.LogInformation("Generated receipt number: {ReceiptNumber}", receiptNumber);
        return receiptNumber;
    }

    public async Task<string> GetReceiptTemplateAsync(CancellationToken cancellationToken = default)
    {
        return @"
╔══════════════════════════════════════════════════════════════╗
║                        TOSS ERP                              ║
║                     SALES RECEIPT                            ║
╠══════════════════════════════════════════════════════════════╣
║ Receipt: {RECEIPT_NUMBER}                                    ║
║ Date: {DATE}                    Time: {TIME}                 ║
║ Till: {TILL_NAME}                                            ║
║ Customer: {CUSTOMER_NAME}                                    ║
╠══════════════════════════════════════════════════════════════╣
║ ITEM                    QTY    PRICE     TAX      TOTAL      ║
╠══════════════════════════════════════════════════════════════╣
{ITEMS}
╠══════════════════════════════════════════════════════════════╣
║ Subtotal:                                    {SUBTOTAL}     ║
║ Tax:                                         {TAX_AMOUNT}   ║
║ Discount:                                    {DISCOUNT_AMOUNT} ║
║ TOTAL:                                       {TOTAL}        ║
╠══════════════════════════════════════════════════════════════╣
{PAYMENT_METHODS}
╠══════════════════════════════════════════════════════════════╣
║ {THANK_YOU}                                                 ║
║ Please keep this receipt for your records                   ║
╚══════════════════════════════════════════════════════════════╝";
    }

    private async Task<string> GetTillNameAsync(Guid tillId, CancellationToken cancellationToken)
    {
        var till = await _tillRepository.GetByIdAsync(tillId, cancellationToken);
        return till?.Name ?? "Unknown Till";
    }

    private string GenerateItemsSection(Sale sale)
    {
        var itemsSection = "";
        foreach (var item in sale.Items)
        {
            var itemLine = $"║ {item.ItemName,-20} {item.Quantity,4} {item.UnitPrice,8:F2} {item.TaxAmount,8:F2} {item.TotalAmount,8:F2} ║";
            itemsSection += itemLine + Environment.NewLine;
        }
        return itemsSection;
    }

    private string GeneratePaymentMethodsSection(Sale sale)
    {
        var paymentSection = "";
        foreach (var payment in sale.Payments)
        {
            var paymentLine = $"║ {payment.Method,-15} {payment.Amount,15:F2} {payment.Reference,-20} ║";
            paymentSection += paymentLine + Environment.NewLine;
        }
        return paymentSection;
    }
}
