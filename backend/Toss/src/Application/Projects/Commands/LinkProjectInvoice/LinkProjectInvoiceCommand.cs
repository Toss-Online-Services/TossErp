using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Entities.Projects;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Projects.Commands.LinkProjectInvoice;

public record LinkProjectInvoiceCommand : IRequest<bool>
{
    public int ProjectId { get; init; }
    public int InvoiceId { get; init; }
}

public class LinkProjectInvoiceCommandHandler : IRequestHandler<LinkProjectInvoiceCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public LinkProjectInvoiceCommandHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<bool> Handle(LinkProjectInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId
                && p.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (project == null)
        {
            return false;
        }

        // Validate invoice exists and is an invoice type
        var invoice = await _context.SalesDocuments
            .Include(i => i.Sale)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId
                && i.DocumentType == SalesDocumentType.Invoice
                && i.Sale.Shop.BusinessId == _businessContext.CurrentBusinessId!.Value, cancellationToken);

        if (invoice == null)
        {
            throw new NotFoundException("SalesDocument", request.InvoiceId.ToString());
        }

        // Link the invoice to the project
        project.InvoiceId = request.InvoiceId;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

