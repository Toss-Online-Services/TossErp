namespace TossErp.Accounting.Application.Common.DTOs;

/// <summary>
/// Result summary of an auto reconciliation run
/// </summary>
public class ReconciliationResultDto
{
    public int TotalConsidered { get; set; }
    public int PairsReconciled { get; set; }
    public int RemainingUnreconciled { get; set; }
    public List<Guid> SampleUnmatched { get; set; } = new();
}
