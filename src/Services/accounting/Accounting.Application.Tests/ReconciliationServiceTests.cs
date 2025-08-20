using Moq;
using Microsoft.Extensions.Logging;
using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Application.Services;
using TossErp.Accounting.Domain.Entities;
using TossErp.Accounting.Domain.Enums;
using TossErp.Accounting.Domain.Common;
using Xunit;

namespace TossErp.Accounting.Application.Tests;

public class ReconciliationServiceTests
{
    private readonly Mock<ICashbookEntryRepository> _repo = new();
    private readonly Mock<ILogger<ReconciliationService>> _logger = new();
    private readonly ReconciliationService _service;
    private readonly List<CashbookEntry> _store = new();
    private readonly string _tenant = "tenant-001";

    public ReconciliationServiceTests()
    {
        _service = new ReconciliationService(_repo.Object, _logger.Object);
        _repo.Setup(r => r.GetUnreconciledAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _store.Where(e => !e.IsReconciled));
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => _store.FirstOrDefault(e => e.Id == id));
        _repo.Setup(r => r.UpdateAsync(It.IsAny<CashbookEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CashbookEntry e, CancellationToken _) => e);
    }

    private CashbookEntry Entry(DateTime date, decimal amount, EntryType type)
        => CashbookEntry.Create(date, $"REF-{Guid.NewGuid():N}".Substring(0,10), "Test", new Money(amount, "ZAR"), type, EntryCategory.Sale, Guid.NewGuid(), _tenant);

    [Fact]
    public async Task AutoReconcileAsync_ShouldMatchSimplePair()
    {
        var from = new DateTime(2025,1,1);
        var to = new DateTime(2025,1,31);
        _store.Clear();
        var debit = Entry(from.AddDays(2), 100m, EntryType.Debit);
        var credit = Entry(from.AddDays(3), 100m, EntryType.Credit);
        _store.AddRange(new[]{debit, credit});
        var result = await _service.AutoReconcileAsync(from, to, "tester");
        Assert.Equal(1, result.PairsReconciled);
        Assert.True(debit.IsReconciled && credit.IsReconciled);
        Assert.Equal(0, result.RemainingUnreconciled);
    }

    [Fact]
    public async Task AutoReconcileAsync_ShouldLeaveWhenAmountOutsideTolerance()
    {
        var from = new DateTime(2025,1,1);
        var to = new DateTime(2025,1,31);
        _store.Clear();
        var debit = Entry(from.AddDays(2), 100m, EntryType.Debit);
        var credit = Entry(from.AddDays(3), 100.05m, EntryType.Credit);
        _store.AddRange(new[]{debit, credit});
        var result = await _service.AutoReconcileAsync(from, to, "tester");
        Assert.Equal(0, result.PairsReconciled);
        Assert.False(debit.IsReconciled || credit.IsReconciled);
        Assert.Equal(2, result.RemainingUnreconciled);
    }

    [Fact]
    public async Task ReconcilePairAsync_ShouldValidateAndMark()
    {
        _store.Clear();
        var debit = Entry(DateTime.UtcNow, 50m, EntryType.Debit);
        var credit = Entry(DateTime.UtcNow, 50m, EntryType.Credit);
        _store.AddRange(new[]{debit, credit});
        await _service.ReconcilePairAsync(debit.Id, credit.Id, "tester");
        Assert.True(debit.IsReconciled && credit.IsReconciled);
    }

    [Fact]
    public async Task AutoReconcileAsync_ShouldPickClosestDateWithinTolerance()
    {
        var from = new DateTime(2025,1,1);
        var to = new DateTime(2025,1,31);
        _store.Clear();
        var debit = Entry(from.AddDays(10), 200m, EntryType.Debit);
        var creditFar = Entry(from.AddDays(13), 200m, EntryType.Credit); // 3 days diff (outside tolerance of 2)
        var creditNear = Entry(from.AddDays(11), 200m, EntryType.Credit); // 1 day diff
        _store.AddRange(new[]{debit, creditFar, creditNear});
        var result = await _service.AutoReconcileAsync(from, to, "tester");
        Assert.Equal(1, result.PairsReconciled);
        Assert.True(debit.IsReconciled && creditNear.IsReconciled);
        Assert.False(creditFar.IsReconciled);
    }

    [Fact]
    public async Task AutoReconcileAsync_ShouldMatchWithinAmountTolerance()
    {
        var from = new DateTime(2025,2,1);
        var to = new DateTime(2025,2,28);
        _store.Clear();
        var debit = Entry(from.AddDays(2), 100.004m, EntryType.Debit); // rounds to 100.00
        var credit = Entry(from.AddDays(3), 100.006m, EntryType.Credit); // rounds to 100.01 (difference 0.01)
        _store.AddRange(new[]{debit, credit});
        var result = await _service.AutoReconcileAsync(from, to, "tester");
        Assert.Equal(1, result.PairsReconciled);
    }

    [Fact]
    public async Task ReconcilePairAsync_ShouldRejectSameType()
    {
        _store.Clear();
        var a = Entry(DateTime.UtcNow, 10m, EntryType.Debit);
        var b = Entry(DateTime.UtcNow, 10m, EntryType.Debit);
        _store.AddRange(new[]{a,b});
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReconcilePairAsync(a.Id, b.Id, "tester"));
    }

    [Fact]
    public async Task ReconcilePairAsync_ShouldRejectAmountBeyondTolerance()
    {
        _store.Clear();
        var a = Entry(DateTime.UtcNow, 10m, EntryType.Debit);
        var b = Entry(DateTime.UtcNow, 10.02m, EntryType.Credit); // 0.02 > 0.01 tolerance
        _store.AddRange(new[]{a,b});
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ReconcilePairAsync(a.Id, b.Id, "tester"));
    }

    [Fact]
    public async Task UnreconcileAsync_ShouldRestoreState()
    {
        _store.Clear();
        var debit = Entry(DateTime.UtcNow, 50m, EntryType.Debit);
        var credit = Entry(DateTime.UtcNow, 50m, EntryType.Credit);
        _store.AddRange(new[]{debit, credit});
        await _service.ReconcilePairAsync(debit.Id, credit.Id, "tester");
        Assert.True(debit.IsReconciled && credit.IsReconciled);
        await _service.UnreconcileAsync(debit.Id, "tester");
        Assert.False(debit.IsReconciled);
        Assert.True(credit.IsReconciled); // only one unreconciled
    }
}
