using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Application.Common.Models;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetDebtors;

public record GetDebtorsQuery : IRequest<PaginatedList<DebtorDto>>
{
    public bool? OverdueOnly { get; init; }
    public int? CustomerId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public record DebtorDto
{
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerEmail { get; init; }
    public string? CustomerPhone { get; init; }
    public decimal TotalOutstanding { get; init; }
    public int InvoiceCount { get; init; }
    public DateTimeOffset? OldestInvoiceDate { get; init; }
    public DateTimeOffset? LatestDueDate { get; init; }
    public List<OutstandingInvoiceDto> OutstandingInvoices { get; init; } = new();
}

public record OutstandingInvoiceDto
{
    public int InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTimeOffset InvoiceDate { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public decimal Amount { get; init; }
    public int DaysOverdue { get; init; }
    public bool IsOverdue { get; init; }
}

public class GetDebtorsQueryHandler : IRequestHandler<GetDebtorsQuery, PaginatedList<DebtorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetDebtorsQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<PaginatedList<DebtorDto>> Handle(GetDebtorsQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;
        var now = DateTimeOffset.UtcNow;

        // Get all unpaid invoices
        var invoicesQuery = _context.SalesDocuments
            .Include(i => i.Customer)
            .Where(i => i.DocumentType == SalesDocumentType.Invoice
                && !i.IsPaid
                && i.Customer != null
                && i.Sale.Shop.BusinessId == businessId);

        if (request.CustomerId.HasValue)
        {
            invoicesQuery = invoicesQuery.Where(i => i.CustomerId == request.CustomerId.Value);
        }

        if (request.OverdueOnly == true)
        {
            invoicesQuery = invoicesQuery.Where(i => i.DueDate.HasValue && i.DueDate < now);
        }

        // Group by customer
        var customerGroups = await invoicesQuery
            .GroupBy(i => new
            {
                CustomerId = i.CustomerId!.Value,
                CustomerName = i.Customer!.FullName ?? i.Customer.Email ?? "Unknown",
                CustomerEmail = i.Customer.Email,
                CustomerPhone = i.Customer.Phone != null ? i.Customer.Phone.ToString() : null
            })
            .Select(g => new DebtorDto
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                CustomerEmail = g.Key.CustomerEmail,
                CustomerPhone = g.Key.CustomerPhone,
                TotalOutstanding = g.Sum(i => i.TotalAmount),
                InvoiceCount = g.Count(),
                OldestInvoiceDate = g.Min(i => i.DocumentDate),
                LatestDueDate = g.Max(i => i.DueDate),
                OutstandingInvoices = g.Select(i => new OutstandingInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.DocumentNumber,
                    InvoiceDate = i.DocumentDate,
                    DueDate = i.DueDate,
                    Amount = i.TotalAmount,
                    DaysOverdue = i.DueDate.HasValue && i.DueDate < now
                        ? (int)(now - i.DueDate.Value).TotalDays
                        : 0,
                    IsOverdue = i.DueDate.HasValue && i.DueDate < now
                }).OrderByDescending(inv => inv.DueDate ?? inv.InvoiceDate).ToList()
            })
            .OrderByDescending(d => d.TotalOutstanding)
            .ToListAsync(cancellationToken);

        // Apply pagination
        var totalCount = customerGroups.Count;
        var pagedDebtors = customerGroups
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PaginatedList<DebtorDto>(pagedDebtors, totalCount, request.PageNumber, request.PageSize);
    }
}

