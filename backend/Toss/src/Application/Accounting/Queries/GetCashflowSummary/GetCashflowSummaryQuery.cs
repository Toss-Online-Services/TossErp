using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetCashflowSummary;

public record GetCashflowSummaryQuery : IRequest<CashflowSummaryDto>
{
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public int? AccountId { get; init; }
}

public record CashflowSummaryDto
{
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public decimal OpeningBalance { get; init; }
    public decimal TotalCashIn { get; init; }
    public decimal TotalCashOut { get; init; }
    public decimal ClosingBalance { get; init; }
    public List<CashflowItemDto> CashInItems { get; init; } = new();
    public List<CashflowItemDto> CashOutItems { get; init; } = new();
}

public record CashflowItemDto
{
    public string Source { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Count { get; init; }
    public DateTimeOffset? LastEntryDate { get; init; }
}

public class GetCashflowSummaryQueryHandler : IRequestHandler<GetCashflowSummaryQuery, CashflowSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetCashflowSummaryQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<CashflowSummaryDto> Handle(GetCashflowSummaryQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        // Calculate opening balance (sum of all entries before FromDate)
        var openingBalanceQuery = _context.CashbookEntries
            .Where(e => e.BusinessId == businessId
                && e.EntryDate < request.FromDate);

        if (request.AccountId.HasValue)
        {
            openingBalanceQuery = openingBalanceQuery.Where(e => e.AccountId == request.AccountId.Value);
        }

        var openingBalance = await openingBalanceQuery
            .SumAsync(e => e.Amount, cancellationToken);

        // Calculate cash in (positive amounts) within period
        var cashInQuery = _context.CashbookEntries
            .Where(e => e.BusinessId == businessId
                && e.Amount > 0
                && e.EntryDate >= request.FromDate
                && e.EntryDate <= request.ToDate);

        if (request.AccountId.HasValue)
        {
            cashInQuery = cashInQuery.Where(e => e.AccountId == request.AccountId.Value);
        }

        var cashInItems = await cashInQuery
            .GroupBy(e => e.SourceType ?? "CashIn")
            .Select(g => new CashflowItemDto
            {
                Source = g.Key,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count(),
                LastEntryDate = g.Max(e => e.EntryDate)
            })
            .ToListAsync(cancellationToken);

        var totalCashIn = cashInItems.Sum(i => i.Amount);

        // Calculate cash out (negative amounts) within period
        var cashOutQuery = _context.CashbookEntries
            .Where(e => e.BusinessId == businessId
                && e.Amount < 0
                && e.EntryDate >= request.FromDate
                && e.EntryDate <= request.ToDate);

        if (request.AccountId.HasValue)
        {
            cashOutQuery = cashOutQuery.Where(e => e.AccountId == request.AccountId.Value);
        }

        var cashOutItems = await cashOutQuery
            .GroupBy(e => e.SourceType ?? "Expense")
            .Select(g => new CashflowItemDto
            {
                Source = g.Key,
                Amount = Math.Abs(g.Sum(e => e.Amount)), // Make positive for display
                Count = g.Count(),
                LastEntryDate = g.Max(e => e.EntryDate)
            })
            .ToListAsync(cancellationToken);

        var totalCashOut = cashOutItems.Sum(i => i.Amount);

        var closingBalance = openingBalance + totalCashIn - totalCashOut;

        return new CashflowSummaryDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            OpeningBalance = openingBalance,
            TotalCashIn = totalCashIn,
            TotalCashOut = totalCashOut,
            ClosingBalance = closingBalance,
            CashInItems = cashInItems,
            CashOutItems = cashOutItems
        };
    }
}

