using System;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.DTOs;

public class MovementDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public string ItemCode { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public Guid? BinId { get; set; }
    public string? BinCode { get; set; }
    public string MovementType { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
    public DateTime MovementDate { get; set; }
    public Guid? BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
