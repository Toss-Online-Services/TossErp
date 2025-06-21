namespace TossErp.Application.DTOs;

public class GroupPurchaseDto
{
    public Guid Id { get; set; }
    public string GroupNumber { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal GroupPrice { get; set; }
    public int MinimumQuantity { get; set; }
    public int TargetQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string? Description { get; set; }
    public string? DeliveryLocation { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public int MemberCount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime EndDate { get; set; }
}

public class CreateGroupPurchaseDto
{
    public Guid BusinessId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal GroupPrice { get; set; }
    public int MinimumQuantity { get; set; }
    public int TargetQuantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid CreatedBy { get; set; }
    public string? Description { get; set; }
    public string? DeliveryLocation { get; set; }
    public string? ProductDetails { get; set; }
} 
