using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Toss.Application.Common.Interfaces;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.Sales.Commands.CreateSalesDocument;

public record CreateSalesDocumentCommand : IRequest<int>
{
    public int SaleId { get; init; }
    public SalesDocumentType DocumentType { get; init; }
    public string? DocumentNumber { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public string? Notes { get; init; }
}

public class CreateSalesDocumentCommandHandler : IRequestHandler<CreateSalesDocumentCommand, int>                                                               
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSalesDocumentCommandHandler> _logger;

    public CreateSalesDocumentCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateSalesDocumentCommandHandler> logger)     
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(CreateSalesDocumentCommand request, CancellationToken cancellationToken)                                                      
    {
        // Retry logic to handle race conditions in document number generation
        const int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await CreateDocumentInternal(request, cancellationToken);
            }
            catch (DbUpdateException ex) when (attempt < maxRetries && IsDuplicateKeyException(ex))
            {
                _logger.LogWarning(
                    "Duplicate key error creating document for sale {SaleId}, type {DocumentType}. Retrying (attempt {Attempt}/{MaxRetries})",
                    request.SaleId,
                    request.DocumentType,
                    attempt,
                    maxRetries);
                
                // Wait with exponential backoff before retrying (increased delay)
                await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1)), cancellationToken);
            }
        }

        // Final attempt without catching exception
        return await CreateDocumentInternal(request, cancellationToken);
    }

    private bool IsDuplicateKeyException(DbUpdateException ex)
    {
        // Check if it's a unique constraint violation
        // Works for PostgreSQL (23505), SQL Server (2627/2601), and other databases
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("duplicate key") 
           || message.Contains("unique constraint") 
           || message.Contains("IX_SalesDocuments_DocumentNumber")
           || (ex.InnerException != null && ex.InnerException.GetType().Name.Contains("Postgres"));
    }

    private async Task<int> CreateDocumentInternal(CreateSalesDocumentCommand request, CancellationToken cancellationToken)
    {
        // Check for existing document FIRST (idempotency)
        var existing = await _context.SalesDocuments
            .FirstOrDefaultAsync(d => d.SaleId == request.SaleId && d.DocumentType == request.DocumentType, cancellationToken);                                 
        if (existing != null)
        {
            return existing.Id;
        }

        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);                                                                               

        if (sale == null)
            throw new Toss.Application.Common.Exceptions.NotFoundException(nameof(Sale), request.SaleId.ToString());                                            

        // Validation
        if (request.DocumentType == SalesDocumentType.Invoice && !sale.CustomerId.HasValue)                                                                     
            throw new InvalidOperationException("Sale must have a customer to create invoice");                                                                 

        var number = request.DocumentNumber ?? await GenerateDocumentNumber(request.DocumentType, sale.ShopId, cancellationToken);                              

        var isReceipt = request.DocumentType == SalesDocumentType.Receipt;      
        var isInvoice = request.DocumentType == SalesDocumentType.Invoice;      

        var document = new SalesDocument
        {
            DocumentType = request.DocumentType,
            DocumentNumber = number,
            SaleId = sale.Id,
            CustomerId = sale.CustomerId, // nullable OK for Receipt
            ShopId = sale.ShopId,
            DocumentDate = DateTimeOffset.UtcNow,
            DueDate = isInvoice
                ? (request.DueDate.HasValue ? request.DueDate.Value.ToUniversalTime() : DateTimeOffset.UtcNow.AddDays(30))                                      
                : null,
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            TotalAmount = sale.Total,
            IsPaid = isReceipt,
            PaidDate = isReceipt ? DateTimeOffset.UtcNow : null,
            Notes = request.Notes
        };

        _context.SalesDocuments.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }

    private async Task<string> GenerateDocumentNumber(SalesDocumentType type, int shopId, CancellationToken cancellationToken)
    {
        var date = DateTimeOffset.UtcNow;
        var year = date.Year;
        var prefix = type switch
        {
            SalesDocumentType.Invoice => "INV",
            SalesDocumentType.Receipt => "RCT",
            SalesDocumentType.CreditNote => "CRN",
            _ => "DOC"
        };

        // Use Max(DocumentNumber) approach with timestamp suffix to avoid race condition
        var prefixPattern = $"{prefix}-{year}-";
        var lastDocumentNumber = await _context.SalesDocuments
            .Where(d => d.ShopId == shopId 
                && d.DocumentType == type 
                && d.DocumentDate.Year == year
                && d.DocumentNumber.StartsWith(prefixPattern))
            .OrderByDescending(d => d.DocumentNumber)
            .Select(d => d.DocumentNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextSequence = 1;
        if (!string.IsNullOrEmpty(lastDocumentNumber))
        {
            // Extract sequence number from format: {prefix}-{year}-{sequence}[-timestamp]
            var parts = lastDocumentNumber.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastSequence))
            {
                nextSequence = lastSequence + 1;
            }
        }

        // Add microseconds timestamp to make the number unique in case of race conditions
        var timestamp = DateTimeOffset.UtcNow.Ticks % 1000000; // Last 6 digits of ticks (microseconds)
        return $"{prefixPattern}{nextSequence:D4}-{timestamp:D6}";
    }
}
