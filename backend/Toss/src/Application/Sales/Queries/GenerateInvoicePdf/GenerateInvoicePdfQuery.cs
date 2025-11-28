using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Sales.Queries.GenerateInvoicePdf;

/// <summary>
/// Query to generate a PDF for an invoice.
/// MVP: Returns a placeholder response indicating PDF generation is not yet implemented.
/// Future: Will generate actual PDF using a library like QuestPDF or iTextSharp.
/// </summary>
public record GenerateInvoicePdfQuery : IRequest<InvoicePdfResult>
{
    public int InvoiceId { get; init; }
}

public record InvoicePdfResult
{
    public int InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public bool IsPdfGenerated { get; init; }
    public string? PdfUrl { get; init; }
    public string? Message { get; init; }
}

public class GenerateInvoicePdfQueryHandler : IRequestHandler<GenerateInvoicePdfQuery, InvoicePdfResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GenerateInvoicePdfQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<InvoicePdfResult> Handle(GenerateInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

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

        // MVP: PDF generation not yet implemented
        // Future implementation will:
        // 1. Use a PDF library (e.g., QuestPDF, iTextSharp, or PuppeteerSharp)
        // 2. Generate PDF with invoice details, line items, totals, VAT breakdown
        // 3. Store PDF in blob storage or return as base64
        // 4. Return PDF URL or download link

        return new InvoicePdfResult
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.DocumentNumber,
            IsPdfGenerated = false,
            PdfUrl = null,
            Message = "PDF generation is not yet implemented. Use the invoice JSON data for now."
        };
    }
}

