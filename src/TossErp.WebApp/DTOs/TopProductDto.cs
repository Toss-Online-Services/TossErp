namespace TossErp.WebApp.DTOs
{
    public class TopProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalRevenue { get; set; } // UI alias - made writable
        public decimal AveragePrice { get; set; }
        public int CurrentStock { get; set; }
        public string? ImageUrl { get; set; }
    }
} 
