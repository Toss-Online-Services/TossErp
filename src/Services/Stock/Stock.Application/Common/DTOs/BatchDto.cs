namespace TossErp.Stock.Application.Common.DTOs;

public class BatchDto
{
    public Guid Id { get; set; }
    public string BatchId { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? WarrantyExpiryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal DispatchedQty { get; set; }
    public decimal ReturnedQty { get; set; }
    public decimal ScrappedQty { get; set; }
    public decimal RetainSample { get; set; }
    public decimal RetainSampleQty { get; set; }
    public string RetainSampleUOM { get; set; } = string.Empty;
    public decimal RetainSampleUOMQty { get; set; }
    public string RetainSampleWarehouse { get; set; } = string.Empty;
    public string RetainSampleBin { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
} 
