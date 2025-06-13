#nullable enable
using System;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.ProductAggregate
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Code { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int LowStockThreshold { get; set; }
        public string Brand { get; set; } = string.Empty;
        public int StockLevel { get; set; }

        public void UpdateStock(int quantity)
        {
            StockQuantity += quantity;
            StockLevel = StockQuantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
