using MediatR;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;

namespace TossErp.Sales.Application.Commands.CancelSale;

/// <summary>
/// Handler for cancelling a sale
/// </summary>
public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITillRepository _tillRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;

    public CancelSaleCommandHandler(
        ISaleRepository saleRepository,
        ITillRepository tillRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService)
    {
        _saleRepository = saleRepository;
        _tillRepository = tillRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
    }

    public async Task<SaleDto> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        // Get sale
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.SaleId} not found");

        // Validate sale can be cancelled
        if (sale.Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        if (sale.Status == SaleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed sale. Use refund instead.");

        // Get till for DTO mapping
        var till = await _tillRepository.GetByIdAsync(sale.TillId, cancellationToken);
        if (till == null)
            throw new InvalidOperationException($"Till with ID {sale.TillId} not found");

        // Cancel sale
        sale.Cancel(request.Reason, _currentUserService.UserId ?? "system");

        // Update sale
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Commit transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain events
        if (sale.DomainEvents.Any())
        {
            await _domainEventService.PublishAsync(sale.DomainEvents, cancellationToken);
            sale.ClearDomainEvents();
        }

        // Return DTO
        return MapToDto(sale, till);
    }

    private static SaleDto MapToDto(Sale sale, Till till)
    {
        return new SaleDto
        {
            Id = sale.Id,
            ReceiptNumber = sale.ReceiptNumber.Value,
            TillId = sale.TillId,
            TillName = till.Name,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            Status = sale.Status,
            SubTotal = sale.SubTotal.Amount,
            TaxAmount = sale.TaxAmount.Amount,
            DiscountAmount = sale.DiscountAmount.Amount,
            Total = sale.Total.Amount,
            PaidAmount = sale.PaidAmount.Amount,
            ChangeAmount = sale.ChangeAmount.Amount,
            Notes = sale.Notes,
            CreatedAt = sale.CreatedAt,
            CompletedAt = sale.CompletedAt,
            CancelledAt = sale.CancelledAt,
            CancellationReason = sale.CancellationReason,
            CreatedBy = sale.CreatedBy,
            TenantId = sale.TenantId,
            Items = sale.Items.Select(item => new SaleItemDto
            {
                Id = item.Id,
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                ItemSku = item.ItemSku,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.Amount,
                TaxRate = item.TaxRate,
                LineTotal = item.LineTotal.Amount,
                TaxAmount = item.TaxAmount.Amount,
                LineTotalIncludingTax = item.LineTotalIncludingTax.Amount
            }).ToList(),
            Payments = sale.Payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                Method = payment.Method,
                Amount = payment.Amount.Amount,
                Reference = payment.Reference,
                CardLast4Digits = payment.CardLast4Digits,
                CardType = payment.CardType,
                AuthorizationCode = payment.AuthorizationCode,
                ProcessedAt = payment.ProcessedAt,
                IsSuccessful = payment.IsSuccessful,
                FailureReason = payment.FailureReason
            }).ToList()
        };
    }
}
