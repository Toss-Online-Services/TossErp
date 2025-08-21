using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Application.Common.Exceptions;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Application.Commands.AddPayment;

/// <summary>
/// Handler for AddPaymentCommand
/// </summary>
public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<AddPaymentCommandHandler> _logger;

    public AddPaymentCommandHandler(
        ISaleRepository saleRepository,
        ILogger<AddPaymentCommandHandler> logger)
    {
        _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SaleDto> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding payment to sale {SaleId}: {Method} {Amount}", 
            request.SaleId, request.Method, request.Amount);

        // Get the sale
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new NotFoundException("Sale", request.SaleId.ToString());
        }

        // Add payment
        var paymentAmount = new Money(request.Amount);
        sale.AddPayment(request.Method, paymentAmount, request.Reference);

        // Update sale
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("Payment added successfully to sale {SaleId}", request.SaleId);

        // Map to DTO
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
