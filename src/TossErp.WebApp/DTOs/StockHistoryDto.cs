namespace TossErp.WebApp.DTOs
{
    public class StockHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
        
        // UI alias properties
        public DateTime Date => TransactionDate;
        public string Type => TransactionType;
        public string Reason => Notes ?? string.Empty;
        public string User => UserName;
    }
} 
