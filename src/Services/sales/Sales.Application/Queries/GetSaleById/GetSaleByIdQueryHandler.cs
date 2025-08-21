using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Domain.Common;

namespace TossErp.Sales.Application.Queries.GetSaleById;

/// <summary>
/// Handler for GetSaleByIdQuery
/// </summary>
public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<GetSaleByIdQueryHandler> _logger;

    public GetSaleByIdQueryHandler(
        ISaleRepository saleRepository,
        ILogger<GetSaleByIdQueryHandler> logger)
    {
        _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting sale by ID: {SaleId}", request.SaleId);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        
        if (sale == null)
        {
            _logger.LogWarning("Sale not found: {SaleId}", request.SaleId);
            return null;
        }

        return MapToDto(sale);
    }

    private static SaleDto MapToDto(Domain.Entities.Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            ReceiptNumber = sale.ReceiptNumber.Value,
            TillId = sale.TillId,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            Status = sale.Status,
            SubTotal = sale.SubTotal.Amount,
            TaxAmount = sale.TaxAmount.Amount,
            DiscountAmount = sale.DiscountAmount.Amount,
            DiscountReason = sale.DiscountReason,
            Total = sale.Total.Amount,
            PaidAmount = sale.PaidAmount.Amount,
            ChangeAmount = sale.ChangeAmount.Amount,
            Notes = sale.Notes,
            CompletedAt = sale.CompletedAt,
            CancelledAt = sale.CancelledAt,
            CancellationReason = sale.CancellationReason,
            CreatedAt = sale.CreatedDate,
            CreatedBy = sale.CreatedBy,
            TenantId = sale.TenantId,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                ItemSku = i.ItemSku,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                TaxRate = i.TaxRate,
                LineTotal = i.LineTotal.Amount,
                TaxAmount = i.TaxAmount.Amount,
                LineTotalIncludingTax = i.LineTotal.Amount + i.TaxAmount.Amount
            }).ToList(),
            Payments = sale.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Method = p.Method,
                Amount = p.Amount.Amount,
                Reference = p.Reference,
                ProcessedAt = p.ProcessedAt,
                IsSuccessful = true
            }).ToList()
        };
    }
}
