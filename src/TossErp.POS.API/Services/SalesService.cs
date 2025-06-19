using TossErp.Shared.DTOs;
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
            var sale = new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = GenerateSaleNumber(),
                CustomerId = request.CustomerId,
                CustomerName = "Customer", // Would be fetched from customer service
                Items = request.Items.Select(item => new SaleItemDto
                {
                    ItemId = item.ItemId,
                    ItemName = "Item", // Would be fetched from inventory service
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TotalAmount = (item.UnitPrice * item.Quantity) - item.DiscountAmount
                }).ToList(),
                SubTotal = request.Items.Sum(item => (item.UnitPrice * item.Quantity) - item.DiscountAmount),
                DiscountAmount = request.DiscountAmount,
                TaxAmount = request.TaxAmount,
                TotalAmount = request.Items.Sum(item => (item.UnitPrice * item.Quantity) - item.DiscountAmount) - request.DiscountAmount + request.TaxAmount,
                PaymentMethod = request.PaymentMethod,
                Status = SaleStatus.Pending,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _sales.Add(sale);
            _logger.LogInformation("Created new sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<SaleDto?> GetSaleByIdAsync(Guid id)
        {
            return _sales.FirstOrDefault(s => s.Id == id);
        }

        public async Task<List<SaleDto>> GetSalesAsync(DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
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
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null || sale.Status != SaleStatus.Pending)
            {
                return null;
            }

            sale.Status = SaleStatus.Completed;
            sale.PaymentMethod = request.PaymentMethod;
            sale.AmountPaid = request.AmountPaid;
            sale.TransactionReference = request.TransactionReference;
            sale.Notes = request.Notes;
            sale.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation("Completed sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<SaleDto?> CancelSaleAsync(Guid id, CancelSaleDto request)
        {
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale == null || sale.Status == SaleStatus.Cancelled)
            {
                return null;
            }

            sale.Status = SaleStatus.Cancelled;
            sale.CancellationReason = request.Reason;
            sale.Notes = request.Notes;
            sale.CancelledAt = DateTime.UtcNow;

            _logger.LogInformation("Cancelled sale: {SaleNumber}", sale.SaleNumber);
            return sale;
        }

        public async Task<DailySummaryDto> GetDailySummaryAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            var dailySales = _sales.Where(s => s.CreatedAt >= startOfDay && s.CreatedAt < endOfDay && s.Status == SaleStatus.Completed);

            return new DailySummaryDto
            {
                Date = date,
                TotalSales = dailySales.Count(),
                TotalRevenue = dailySales.Sum(s => s.TotalAmount),
                TotalItems = dailySales.Sum(s => s.Items.Sum(item => item.Quantity)),
                AverageTicketValue = dailySales.Any() ? dailySales.Average(s => s.TotalAmount) : 0,
                PaymentMethods = dailySales.GroupBy(s => s.PaymentMethod)
                    .Select(g => new PaymentMethodSummaryDto
                    {
                        PaymentMethod = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(s => s.TotalAmount)
                    }).ToList()
            };
        }

        public async Task<SalesReportDto> GetSalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            var sales = _sales.Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate && s.Status == SaleStatus.Completed);

            return new SalesReportDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalSales = sales.Count(),
                TotalRevenue = sales.Sum(s => s.TotalAmount),
                TotalItems = sales.Sum(s => s.Items.Sum(item => item.Quantity)),
                AverageTicketValue = sales.Any() ? sales.Average(s => s.TotalAmount) : 0,
                DailyBreakdown = sales.GroupBy(s => s.CreatedAt.Date)
                    .Select(g => new DailyBreakdownDto
                    {
                        Date = g.Key,
                        SalesCount = g.Count(),
                        Revenue = g.Sum(s => s.TotalAmount)
                    }).OrderBy(d => d.Date).ToList()
            };
        }

        private string GenerateSaleNumber()
        {
            return $"SALE-{DateTime.UtcNow:yyyyMMdd}-{_sales.Count + 1:D4}";
        }
    }
} 
