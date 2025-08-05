namespace TossErp.Stock.Application.Common.DTOs;

public class SerialNoDto
{
    public Guid Id { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public string Bin { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal DispatchedQty { get; set; }
    public decimal ReturnedQty { get; set; }
    public decimal ScrappedQty { get; set; }
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
