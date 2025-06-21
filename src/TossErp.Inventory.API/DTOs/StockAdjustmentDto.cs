using TossErp.Shared.Enums;

namespace TossErp.Inventory.API.DTOs
{
    public class StockAdjustmentDto
    {
        public int Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Reference { get; set; }
        public Guid? WarehouseId { get; set; }
    }
} 
