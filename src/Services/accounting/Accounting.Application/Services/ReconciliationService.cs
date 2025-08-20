using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.DTOs;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Common.Helpers;
using TossErp.Accounting.Domain.Entities;

namespace TossErp.Accounting.Application.Services;

public class ReconciliationService : IReconciliationService
{
    private readonly ICashbookEntryRepository _entryRepository;
    private readonly ILogger<ReconciliationService> _logger;

    private const decimal AmountTolerance = 0.01m; // cents tolerance
    private const int DateToleranceDays = 2; // posting vs statement date skew
    private const int UnmatchedSampleLimit = 25;

    public ReconciliationService(ICashbookEntryRepository entryRepository, ILogger<ReconciliationService> logger)
    {
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task<ReconciliationResultDto> AutoReconcileAsync(DateTime fromDate, DateTime toDate, string performedBy, CancellationToken cancellationToken = default)
    {
        if (toDate < fromDate) throw new ArgumentException("toDate must be on or after fromDate");
        var tenantId = "tenant-001"; // TODO: inject tenant context
        var allUnreconciled = await _entryRepository.GetUnreconciledAsync(tenantId, cancellationToken);
        var window = allUnreconciled.Where(e => e.TransactionDate.Date >= fromDate.Date && e.TransactionDate.Date <= toDate.Date).ToList();

    var debits = window.Where(e => e.Type == TossErp.Accounting.Domain.Enums.EntryType.Debit).OrderBy(e => e.TransactionDate).ToList();
    var credits = window.Where(e => e.Type == TossErp.Accounting.Domain.Enums.EntryType.Credit).OrderBy(e => e.TransactionDate).ToList();

        int pairs = 0;
    var creditBuckets = credits.GroupBy(c => c.Amount.Amount.RoundMoney()).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var debit in debits)
        {
            var key = debit.Amount.Amount.RoundMoney();
            if (!creditBuckets.TryGetValue(key, out var bucket) || bucket.Count == 0)
            {
                var altKey = creditBuckets.Keys.FirstOrDefault(k => Math.Abs(k - key) <= AmountTolerance);
                if (altKey != 0 && creditBuckets.TryGetValue(altKey, out var altBucket) && altBucket.Count > 0)
                    bucket = altBucket;
            }
            if (bucket == null || bucket.Count == 0) continue;

            CashbookEntry? candidate = null;
            var minDays = int.MaxValue;
            foreach (var credit in bucket)
            {
                var days = Math.Abs((credit.TransactionDate.Date - debit.TransactionDate.Date).Days);
                if (days <= DateToleranceDays && days < minDays)
                {
                    candidate = credit;
                    minDays = days;
                    if (days == 0) break;
                }
            }
            if (candidate == null) continue;

            debit.MarkAsReconciled(performedBy);
            candidate.MarkAsReconciled(performedBy);
            await _entryRepository.UpdateAsync(debit, cancellationToken);
            await _entryRepository.UpdateAsync(candidate, cancellationToken);
            bucket.Remove(candidate);
            pairs++;
        }

        var remaining = window.Where(e => !e.IsReconciled).Take(UnmatchedSampleLimit).Select(e => e.Id).ToList();

        _logger.LogInformation("Auto reconciliation run completed: Pairs={Pairs} Remaining={Remaining}", pairs, remaining.Count);
        return new ReconciliationResultDto
        {
            TotalConsidered = window.Count,
            PairsReconciled = pairs,
            RemainingUnreconciled = window.Count - (pairs * 2),
            SampleUnmatched = remaining
        };
    }

    public async Task ReconcilePairAsync(Guid firstEntryId, Guid secondEntryId, string performedBy, CancellationToken cancellationToken = default)
    {
        if (firstEntryId == secondEntryId) throw new ArgumentException("Entries must be distinct");
        var a = await _entryRepository.GetByIdAsync(firstEntryId, cancellationToken) ?? throw new KeyNotFoundException("First entry not found");
        var b = await _entryRepository.GetByIdAsync(secondEntryId, cancellationToken) ?? throw new KeyNotFoundException("Second entry not found");
        if (a.IsReconciled || b.IsReconciled) throw new InvalidOperationException("One or both entries already reconciled");
        if (a.Type == b.Type) throw new InvalidOperationException("Entries must be opposite types");
        if (Math.Abs(a.Amount.Amount - b.Amount.Amount) > AmountTolerance) throw new InvalidOperationException("Amounts differ beyond tolerance");
        a.MarkAsReconciled(performedBy);
        b.MarkAsReconciled(performedBy);
        await _entryRepository.UpdateAsync(a, cancellationToken);
        await _entryRepository.UpdateAsync(b, cancellationToken);
        _logger.LogInformation("Manually reconciled pair {A} & {B}", a.Id, b.Id);
    }

    public async Task UnreconcileAsync(Guid entryId, string performedBy, CancellationToken cancellationToken = default)
    {
        var entry = await _entryRepository.GetByIdAsync(entryId, cancellationToken) ?? throw new KeyNotFoundException("Entry not found");
        if (!entry.IsReconciled) throw new InvalidOperationException("Entry not reconciled");
        entry.MarkAsUnreconciled(performedBy);
        await _entryRepository.UpdateAsync(entry, cancellationToken);
        _logger.LogInformation("Entry {EntryId} unreconciled", entryId);
    }
}
