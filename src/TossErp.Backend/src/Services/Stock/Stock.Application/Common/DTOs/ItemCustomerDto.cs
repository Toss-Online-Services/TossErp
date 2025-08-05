namespace TossErp.Stock.Application.Common.DTOs;

public class ItemCustomerDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string CustomerItemCode { get; set; } = string.Empty;
    public string CustomerItemName { get; set; } = string.Empty;
    public decimal? RefRate { get; set; }
    public string? RefCurrency { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
