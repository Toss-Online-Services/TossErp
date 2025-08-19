using MediatR;
using TossErp.Sales.Application.Common.DTOs;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Application.Commands.CreateSale;

/// <summary>
/// Handler for creating a new sale
/// </summary>
public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITillRepository _tillRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;

    public CreateSaleCommandHandler(
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

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Validate till exists and is open
        var till = await _tillRepository.GetByIdAsync(request.TillId, cancellationToken);
        if (till == null)
            throw new InvalidOperationException($"Till with ID {request.TillId} not found");

        if (till.Status != TillStatus.Open)
            throw new InvalidOperationException($"Till {till.Name} is not open. Current status: {till.Status}");

        // Generate receipt number
        var receiptNumber = till.GenerateReceiptNumber();

        // Create sale
        var sale = new Sale(
            Guid.NewGuid(),
            receiptNumber,
            request.TillId,
            _currentUserService.TenantId ?? "default-tenant",
            _currentUserService.UserId ?? "system");

        // Set customer information
        if (request.CustomerId.HasValue || !string.IsNullOrWhiteSpace(request.CustomerName))
        {
            sale.SetCustomer(request.CustomerId, request.CustomerName);
        }

        // Add items
        foreach (var itemRequest in request.Items)
        {
            sale.AddItem(
                itemRequest.ItemId,
                itemRequest.ItemName,
                itemRequest.ItemSku,
                itemRequest.Quantity,
                new Money(itemRequest.UnitPrice),
                itemRequest.TaxRate);
        }

        // Apply discount if specified
        if (request.DiscountAmount.HasValue && request.DiscountAmount.Value > 0)
        {
            sale.ApplyDiscount(new Money(request.DiscountAmount.Value), request.DiscountReason ?? "Discount applied");
        }

        // Add notes if specified
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            sale.AddNotes(request.Notes, _currentUserService.UserId ?? "system");
        }

        // Save sale
        await _saleRepository.AddAsync(sale, cancellationToken);

        // Update till receipt sequence
        await _tillRepository.UpdateAsync(till, cancellationToken);

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
            DiscountReason = sale.DiscountReason,
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
