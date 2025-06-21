using TossErp.POS.API.DTOs;
using TossErp.Shared.Enums;

namespace TossErp.POS.API.Services
{
    public class SalesService : ISalesService
    {
        private readonly List<SaleDto> _sales = new();
        private readonly ILogger<SalesService> _logger;

        public SalesService(ILogger<SalesService> logger)
        {
            _logger = logger;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Initialize sample sales
            var sale1 = new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-001",
                CustomerId = Guid.NewGuid(),
                CustomerName = "John Doe",
                Items = new List<SaleItemDto>
                {
                    new SaleItemDto
                    {
                        ItemId = Guid.NewGuid(),
                        ItemName = "Dell Latitude Laptop",
                        Quantity = 1,
                        UnitPrice = 1200.00m,
                        DiscountAmount = 0,
                        TotalAmount = 1200.00m
                    }
                },
                SubTotal = 1200.00m,
                DiscountAmount = 0,
                TaxAmount = 120.00m,
                TotalAmount = 1320.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                Status = SaleStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                CompletedAt = DateTime.UtcNow.AddDays(-1)
            };

            var sale2 = new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-002",
                CustomerId = Guid.NewGuid(),
                CustomerName = "Jane Smith",
                Items = new List<SaleItemDto>
                {
                    new SaleItemDto
                    {
                        ItemId = Guid.NewGuid(),
                        ItemName = "Wireless Mouse",
                        Quantity = 2,
                        UnitPrice = 25.00m,
                        DiscountAmount = 5.00m,
                        TotalAmount = 45.00m
                    },
                    new SaleItemDto
                    {
                        ItemId = Guid.NewGuid(),
                        ItemName = "Cotton T-Shirt",
                        Quantity = 3,
                        UnitPrice = 15.00m,
                        DiscountAmount = 0,
                        TotalAmount = 45.00m
                    }
                },
                SubTotal = 90.00m,
                DiscountAmount = 5.00m,
                TaxAmount = 8.50m,
                TotalAmount = 93.50m,
                PaymentMethod = PaymentMethod.Cash,
                Status = SaleStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                CompletedAt = DateTime.UtcNow.AddHours(-2)
            };

            _sales.AddRange(new[] { sale1, sale2 });
        }

        public async Task<SaleDto> CreateSaleAsync(CreateSaleDto request)
        {
            await Task.CompletedTask;
            
            var subTotal = request.Items.Sum(item => (item.UnitPrice * item.Quantity) - (item.DiscountAmount ?? 0));
            var totalDiscount = request.Items.Sum(item => item.DiscountAmount ?? 0);
            var taxAmount = subTotal * 0.10m; // 10% tax rate
            var totalAmount = subTotal + taxAmount;
            
            var sale = new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = GenerateSaleNumber(),
                CustomerId = request.CustomerId,
                CashierId = request.CashierId,
                CustomerName = "Customer", // Would be fetched from customer service
                CashierName = "Cashier", // Would be fetched from user service
                Items = request.Items.Select(item => new SaleItemDto
                {
                    Id = Guid.NewGuid(),
                    ItemId = item.ItemId,
                    ItemName = item.ItemName ?? "Item", // Would be fetched from inventory service
                    ItemCode = "CODE", // Would be fetched from inventory service
                    Quantity = (int)item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount ?? 0,
                    DiscountPercentage = item.DiscountPercentage ?? 0,
                    TotalAmount = (item.UnitPrice * item.Quantity) - (item.DiscountAmount ?? 0)
                }).ToList(),
                SubTotal = subTotal,
                DiscountAmount = totalDiscount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                PaymentMethod = request.PaymentMethod.HasValue ? request.PaymentMethod.Value : PaymentMethod.Cash,
                Status = SaleStatus.Pending,
                SaleType = request.SaleType,
                Notes = request.Notes,
                SaleDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Payments = new List<SalePaymentDto>()
            };

            _sales.Add(sale);
            _logger.LogInformation("Created new sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<SaleDto?> GetSaleByIdAsync(Guid id)
        {
            await Task.CompletedTask;
            return _sales.FirstOrDefault(s => s.Id == id);
        }

        public async Task<List<SaleDto>> GetSalesAsync(DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            await Task.CompletedTask;
            
            var query = _sales.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= toDate.Value);
            }

            return query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<SaleDto?> CompleteSaleAsync(Guid id, CompleteSaleDto request)
        {
            await Task.CompletedTask;
            
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null || sale.Status != SaleStatus.Pending)
            {
                return null;
            }

            sale.Status = SaleStatus.Completed;
            sale.PaymentMethod = request.PaymentMethod;
            sale.AmountPaid = request.AmountPaid;
            sale.ReferenceNumber = request.ReferenceNumber;
            sale.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation("Completed sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<SaleDto?> CancelSaleAsync(Guid id, CancelSaleDto request)
        {
            await Task.CompletedTask;
            
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null || sale.Status == SaleStatus.Cancelled)
            {
                return null;
            }

            sale.Status = SaleStatus.Cancelled;
            sale.CancellationReason = request.CancellationReason;
            sale.CancelledAt = DateTime.UtcNow;

            _logger.LogInformation("Cancelled sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<DailySummaryDto> GetDailySummaryAsync(DateTime date)
        {
            await Task.CompletedTask;
            
            var dailySales = _sales.Where(s => s.CreatedAt.Date == date.Date && s.Status == SaleStatus.Completed).ToList();
            
            return new DailySummaryDto
            {
                Date = date,
                TotalTransactions = dailySales.Count,
                TotalSales = dailySales.Sum(s => s.TotalAmount),
                CashSales = dailySales.Where(s => s.PaymentMethod == PaymentMethod.Cash).Sum(s => s.TotalAmount),
                CardSales = dailySales.Where(s => s.PaymentMethod == PaymentMethod.CreditCard).Sum(s => s.TotalAmount),
                MobileMoneySales = dailySales.Where(s => s.PaymentMethod == PaymentMethod.MobilePayment).Sum(s => s.TotalAmount),
                OtherPaymentSales = dailySales.Where(s => s.PaymentMethod != PaymentMethod.Cash && s.PaymentMethod != PaymentMethod.CreditCard && s.PaymentMethod != PaymentMethod.MobilePayment).Sum(s => s.TotalAmount),
                AverageTransactionValue = dailySales.Any() ? dailySales.Average(s => s.TotalAmount) : 0,
                ItemsSold = dailySales.Sum(s => s.Items.Sum(item => item.Quantity)),
                TotalDiscounts = dailySales.Sum(s => s.DiscountAmount),
                TotalTax = dailySales.Sum(s => s.TaxAmount)
            };
        }

        public async Task<SalesReportDto> GetSalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            
            var periodSales = _sales.Where(s => s.CreatedAt.Date >= fromDate.Date && s.CreatedAt.Date <= toDate.Date && s.Status == SaleStatus.Completed).ToList();
            
            return new SalesReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalTransactions = periodSales.Count,
                TotalSales = periodSales.Sum(s => s.TotalAmount),
                AverageTransactionValue = periodSales.Any() ? periodSales.Average(s => s.TotalAmount) : 0,
                TopSellingItems = periodSales.SelectMany(s => s.Items)
                    .GroupBy(item => item.ItemName)
                    .Select(g => new SalesReportItemDto
                    {
                        Name = g.Key,
                        Quantity = g.Sum(item => item.Quantity),
                        TotalAmount = g.Sum(item => item.TotalAmount),
                        Percentage = periodSales.Sum(s => s.Items.Sum(item => item.TotalAmount)) > 0 ? 
                            (g.Sum(item => item.TotalAmount) / periodSales.Sum(s => s.Items.Sum(item => item.TotalAmount))) * 100 : 0
                    })
                    .OrderByDescending(item => item.TotalAmount)
                    .Take(10)
                    .ToList(),
                TopSellingCategories = new List<SalesReportItemDto>(), // Would be populated from category data
                CashSales = periodSales.Where(s => s.PaymentMethod == PaymentMethod.Cash).Sum(s => s.TotalAmount),
                CardSales = periodSales.Where(s => s.PaymentMethod == PaymentMethod.CreditCard).Sum(s => s.TotalAmount),
                MobileMoneySales = periodSales.Where(s => s.PaymentMethod == PaymentMethod.MobilePayment).Sum(s => s.TotalAmount),
                OtherPaymentSales = periodSales.Where(s => s.PaymentMethod != PaymentMethod.Cash && s.PaymentMethod != PaymentMethod.CreditCard && s.PaymentMethod != PaymentMethod.MobilePayment).Sum(s => s.TotalAmount)
            };
        }

        private string GenerateSaleNumber()
        {
            return $"SALE-{DateTime.UtcNow:yyyyMMdd}-{_sales.Count + 1:D4}";
        }
    }
} 
