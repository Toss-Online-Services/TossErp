using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; }
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
