using TossErp.Shared.Enums;

namespace TossErp.Shared.DTOs
{
    public class CompleteSaleDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }
    }
} 
