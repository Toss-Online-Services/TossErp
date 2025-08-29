using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Filter criteria for payment queries
/// </summary>
public class PaymentFilter
{
    public Guid? CustomerId { get; set; }
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentNumber { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Reference { get; set; }
    public string? Currency { get; set; }
    public bool? IsAllocated { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = "PaymentDate";
    public bool SortDescending { get; set; } = true;
}
