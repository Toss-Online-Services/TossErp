using MediatR;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;

namespace TossErp.Accounting.Application.Queries.GetCashbookSummary;

/// <summary>
/// Handler for GetCashbookSummaryQuery
/// </summary>
public class GetCashbookSummaryQueryHandler : IRequestHandler<GetCashbookSummaryQuery, CashbookSummaryResponse>
{
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly ILogger<GetCashbookSummaryQueryHandler> _logger;

    public GetCashbookSummaryQueryHandler(
        ICashbookEntryRepository entryRepository,
        ILogger<GetCashbookSummaryQueryHandler> logger)
    {
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task<CashbookSummaryResponse> Handle(GetCashbookSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting cashbook summary as of {AsOfDate}", request.AsOfDate);

        // For MVP, we'll use a hardcoded tenant ID
        // In a real implementation, this would come from the current user context
        var tenantId = "tenant-001";

        // Get all entries for the tenant up to the as-of date
        var allEntries = await _entryRepository.GetByTenantAsync(tenantId, cancellationToken);
        var entriesUpToDate = allEntries.Where(e => e.TransactionDate.Date <= request.AsOfDate.Date).ToList();

        // Calculate totals
        var totalDebits = entriesUpToDate
            .Where(e => e.Type == Domain.Enums.EntryType.Debit)
            .Sum(e => e.Amount.Amount);

        var totalCredits = entriesUpToDate
            .Where(e => e.Type == Domain.Enums.EntryType.Credit)
            .Sum(e => e.Amount.Amount);

        var netBalance = totalDebits - totalCredits;

        // Calculate category totals
        var categoryTotals = entriesUpToDate
            .GroupBy(e => e.Category.ToString())
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.Type == Domain.Enums.EntryType.Debit ? e.Amount.Amount : -e.Amount.Amount)
            );

        // Calculate type totals
        var typeTotals = entriesUpToDate
            .GroupBy(e => e.Type.ToString())
            .ToDictionary(
                g => g.Key,
                g => g.Sum(e => e.Amount.Amount)
            );

        var response = new CashbookSummaryResponse
        {
            AsOfDate = request.AsOfDate,
            TotalDebits = totalDebits,
            TotalCredits = totalCredits,
            NetBalance = netBalance,
            Currency = "ZAR",
            TotalEntries = entriesUpToDate.Count,
            CategoryTotals = categoryTotals,
            TypeTotals = typeTotals
        };

        _logger.LogInformation("Retrieved cashbook summary: TotalDebits={TotalDebits}, TotalCredits={TotalCredits}, NetBalance={NetBalance}, TotalEntries={TotalEntries}", 
            totalDebits, totalCredits, netBalance, entriesUpToDate.Count);

        return response;
    }
}

