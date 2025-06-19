using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
{
    public class StockAdjustmentDto
    {
        public int Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; }
        public Guid? WarehouseId { get; set; }
    }
} 
