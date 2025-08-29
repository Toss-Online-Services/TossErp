using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Filter criteria for subscription queries
/// </summary>
public class SubscriptionFilter
{
    public Guid? CustomerId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public SubscriptionStatus? Status { get; set; }
    public string? PlanName { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = "StartDate";
    public bool SortDescending { get; set; } = true;
}
