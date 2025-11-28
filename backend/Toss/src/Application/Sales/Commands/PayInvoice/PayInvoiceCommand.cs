using Toss.Application.Accounting.Commands.RecordCashIn;
using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Payments.Commands.RecordPayment;
using Toss.Domain.Entities.Accounting;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Sales.Commands.PayInvoice;

public record PayInvoiceCommand : IRequest<PayInvoiceResult>
{
    public int InvoiceId { get; init; }
    public int AccountId { get; init; }
    public PaymentType PaymentType { get; init; }
    public string? TransactionRef { get; init; }
    public string? Notes { get; init; }
}

public record PayInvoiceResult
{
    public int InvoiceId { get; init; }
    public int PaymentId { get; init; }
    public int CashbookEntryId { get; init; }
    public decimal Amount { get; init; }
    public bool IsFullyPaid { get; init; }
}

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, PayInvoiceResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;
    private readonly ISender _sender;

    public PayInvoiceCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext,
        ISender sender)
    {
        _context = context;
        _businessContext = businessContext;
        _sender = sender;
    }

    public async Task<PayInvoiceResult> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        // Load invoice with related data
        var invoice = await _context.SalesDocuments
            .Include(d => d.Sale)
            .ThenInclude(s => s.Shop)
            .FirstOrDefaultAsync(
                d => d.Id == request.InvoiceId 
                    && d.DocumentType == SalesDocumentType.Invoice
                    && d.Shop!.BusinessId == _businessContext.CurrentBusinessId,
                cancellationToken);

        if (invoice == null)
        {
            throw new NotFoundException($"Invoice with ID {request.InvoiceId} not found.");
        }

        if (invoice.IsPaid)
        {
            throw new ValidationException("Invoice is already paid.");
        }

        // Verify account exists and belongs to business
        var account = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == request.AccountId 
                    && a.BusinessId == _businessContext.CurrentBusinessId
                    && a.IsActive,
                cancellationToken);

        if (account == null)
        {
            throw new NotFoundException($"Account with ID {request.AccountId} not found or inactive.");
        }

        // For MVP, only support full payment
        var paymentAmount = invoice.TotalAmount;

        // Check if payment already exists (idempotency check)
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(
                p => p.SaleId == invoice.SaleId 
                    && p.Amount == paymentAmount
                    && p.Status == PaymentStatus.Completed,
                cancellationToken);

        if (existingPayment != null)
        {
            // Payment already exists, find associated cashbook entry
            var existingEntry = await _context.CashbookEntries
                .FirstOrDefaultAsync(
                    e => e.PaymentId == existingPayment.Id 
                        && e.SourceType == "SalesDocument" 
                        && e.SourceId == invoice.Id,
                    cancellationToken);

            if (existingEntry != null && invoice.IsPaid)
            {
                // Already fully processed
                return new PayInvoiceResult
                {
                    InvoiceId = invoice.Id,
                    PaymentId = existingPayment.Id,
                    CashbookEntryId = existingEntry.Id,
                    Amount = paymentAmount,
                    IsFullyPaid = true
                };
            }
        }

        // Create Payment
        var paymentId = await _sender.Send(new RecordPaymentCommand
        {
            ShopId = invoice.Sale.ShopId,
            SaleId = invoice.SaleId,
            Amount = paymentAmount,
            PaymentType = request.PaymentType,
            TransactionRef = request.TransactionRef,
            Notes = request.Notes
        }, cancellationToken);

        // Create CashbookEntry
        var cashbookEntryId = await _sender.Send(new RecordCashInCommand
        {
            AccountId = request.AccountId,
            Amount = paymentAmount,
            EntryDate = DateTimeOffset.UtcNow,
            Reference = invoice.DocumentNumber,
            Notes = request.Notes ?? $"Payment for invoice {invoice.DocumentNumber}",
            SourceType = "SalesDocument",
            SourceId = invoice.Id,
            PaymentId = paymentId
        }, cancellationToken);

        // Update invoice as paid
        invoice.IsPaid = true;
        invoice.PaidDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new PayInvoiceResult
        {
            InvoiceId = invoice.Id,
            PaymentId = paymentId,
            CashbookEntryId = cashbookEntryId,
            Amount = paymentAmount,
            IsFullyPaid = true
        };
    }
}

