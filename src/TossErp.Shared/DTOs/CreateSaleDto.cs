using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
{
    public class CreateSaleDto
    {
        public Guid CustomerId { get; set; }
        public List<CreateSaleItemDto> Items { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateSaleItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
    }
} 
