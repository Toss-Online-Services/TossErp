using TossErp.Inventory.Domain.Enums;

namespace TossErp.Inventory.API.DTOs
{
    public class StockAdjustmentDto
    {
        public Guid ItemId { get; set; }
        public StockMovementType AdjustmentType { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
} 
