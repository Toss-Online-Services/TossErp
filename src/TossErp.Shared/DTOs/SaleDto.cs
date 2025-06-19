using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string? CustomerName { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string Status { get; set; } = "";
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class SaleItemDto
    {
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
} 
