using TossErp.Accounting.Application.Common.DTOs;

namespace TossErp.Accounting.Application.Common.Interfaces;

public interface IReconciliationService
{
    Task<ReconciliationResultDto> AutoReconcileAsync(DateTime fromDate, DateTime toDate, string performedBy, CancellationToken cancellationToken = default);
    Task ReconcilePairAsync(Guid firstEntryId, Guid secondEntryId, string performedBy, CancellationToken cancellationToken = default);
    Task UnreconcileAsync(Guid entryId, string performedBy, CancellationToken cancellationToken = default);
}
