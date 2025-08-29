using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Filter criteria for invoice queries
/// </summary>
public class InvoiceFilter
{
    public Guid? CustomerId { get; set; }
    public DateOnly? IssueDateFrom { get; set; }
    public DateOnly? IssueDateTo { get; set; }
    public DateOnly? DueDateFrom { get; set; }
    public DateOnly? DueDateTo { get; set; }
    public InvoiceStatus? Status { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? CustomerName { get; set; }
    public bool? IsOverdue { get; set; }
    public string? Reference { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string? Currency { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = "IssueDate";
    public bool SortDescending { get; set; } = true;
}
