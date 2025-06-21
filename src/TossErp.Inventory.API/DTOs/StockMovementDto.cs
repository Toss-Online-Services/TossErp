using TossErp.Inventory.Domain.Enums;

namespace TossErp.Inventory.API.DTOs
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public StockMovementType MovementType { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public DateTime MovementDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }
} 
