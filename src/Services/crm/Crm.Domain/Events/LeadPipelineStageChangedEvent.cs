namespace TossErp.CRM.Domain.Events;

/// <summary>
/// Domain event raised when a lead's pipeline stage changes
/// </summary>
public class LeadPipelineStageChangedEvent : DomainEvent
{
    public Guid TenantId { get; }
    public Guid LeadId { get; }
    public string PreviousStage { get; }
    public string NewStage { get; }
    public string ChangedBy { get; }
    public string? Reason { get; }

    public LeadPipelineStageChangedEvent(
        Guid tenantId,
        Guid leadId,
        string previousStage,
        string newStage,
        string changedBy,
        string? reason = null)
    {
        TenantId = tenantId;
        LeadId = leadId;
        PreviousStage = previousStage ?? throw new ArgumentNullException(nameof(previousStage));
        NewStage = newStage ?? throw new ArgumentNullException(nameof(newStage));
        ChangedBy = changedBy ?? throw new ArgumentNullException(nameof(changedBy));
        Reason = reason;
    }
}
