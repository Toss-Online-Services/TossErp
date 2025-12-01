using Toss.Application.Common.Exceptions;
using Toss.Application.Common.Interfaces;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Toss.Application.Accounting.Queries.GetProfitAndLoss;

public record GetProfitAndLossQuery : IRequest<ProfitAndLossDto>
{
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public int? ShopId { get; init; }
}

public record ProfitAndLossDto
{
    public DateTimeOffset FromDate { get; init; }
    public DateTimeOffset ToDate { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal GrossProfit { get; init; }
    public decimal NetProfit { get; init; }
    public List<RevenueItemDto> RevenueItems { get; init; } = new();
    public List<ExpenseItemDto> ExpenseItems { get; init; } = new();
}

public record RevenueItemDto
{
    public string Source { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Count { get; init; }
}

public record ExpenseItemDto
{
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public int Count { get; init; }
}

public class GetProfitAndLossQueryHandler : IRequestHandler<GetProfitAndLossQuery, ProfitAndLossDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessContext _businessContext;

    public GetProfitAndLossQueryHandler(
        IApplicationDbContext context,
        IBusinessContext businessContext)
    {
        _context = context;
        _businessContext = businessContext;
    }

    public async Task<ProfitAndLossDto> Handle(GetProfitAndLossQuery request, CancellationToken cancellationToken)
    {
        if (!_businessContext.HasBusiness)
        {
            throw new ForbiddenAccessException("No business context available.");
        }

        var businessId = _businessContext.CurrentBusinessId!.Value;

        // Calculate Revenue from cashbook entries (Sale type) and invoices
        var revenueQuery = _context.CashbookEntries
            .Where(e => e.BusinessId == businessId
                && e.Type == CashbookEntryType.Sale
                && e.EntryDate >= request.FromDate
                && e.EntryDate <= request.ToDate);

        if (request.ShopId.HasValue)
        {
            // Filter by shop if account is linked to a shop
            revenueQuery = revenueQuery.Where(e => e.Account.StoreId == request.ShopId.Value);
        }

        var revenueEntries = await revenueQuery
            .GroupBy(e => e.SourceType ?? "Sale")
            .Select(g => new RevenueItemDto
            {
                Source = g.Key,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var totalRevenue = revenueEntries.Sum(r => r.Amount);

        // Calculate Expenses from cashbook entries (Expense type)
        var expenseQuery = _context.CashbookEntries
            .Where(e => e.BusinessId == businessId
                && e.Type == CashbookEntryType.Expense
                && e.EntryDate >= request.FromDate
                && e.EntryDate <= request.ToDate);

        if (request.ShopId.HasValue)
        {
            expenseQuery = expenseQuery.Where(e => e.Account.StoreId == request.ShopId.Value);
        }

        var expenseEntries = await expenseQuery
            .GroupBy(e => e.SourceType ?? "Expense")
            .Select(g => new ExpenseItemDto
            {
                Category = g.Key,
                Amount = Math.Abs(g.Sum(e => e.Amount)), // Expenses are negative, so take absolute value
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var totalExpenses = expenseEntries.Sum(e => e.Amount);

        // Gross Profit = Revenue - Cost of Goods Sold (COGS)
        // For MVP, we'll use a simplified approach: Gross Profit = Revenue - Expenses
        // In a full system, COGS would be calculated from purchase costs
        var grossProfit = totalRevenue - totalExpenses;
        var netProfit = grossProfit; // For MVP, no other deductions

        return new ProfitAndLossDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            GrossProfit = grossProfit,
            NetProfit = netProfit,
            RevenueItems = revenueEntries,
            ExpenseItems = expenseEntries
        };
    }
}

