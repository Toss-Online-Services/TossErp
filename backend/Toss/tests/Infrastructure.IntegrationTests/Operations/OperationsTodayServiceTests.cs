using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Toss.Application.Common.Interfaces;
using Toss.Application.Operations.Queries.GetTodayView;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.IntegrationTests;
using Toss.Infrastructure.Services.Operations;

namespace Toss.Tests.Infrastructure.Operations;

public class OperationsTodayServiceTests
{
    [Test]
    public async Task ReturnsEmptySnapshot_WhenBusinessHasNoStores()
    {
        await using var context = CreateContext(nameof(ReturnsEmptySnapshot_WhenBusinessHasNoStores));
        var service = CreateService(context, new DateTimeOffset(2025, 11, 27, 8, 0, 0, TimeSpan.Zero));

        var result = await service.GetTodayAsync(999);

        Assert.That(result.Date, Is.EqualTo(DateOnly.FromDateTime(new DateTime(2025, 11, 27))));
        Assert.That(result.Totals.SalesTotal, Is.EqualTo(0m));
        Assert.That(result.LowStock, Is.Empty);
        Assert.That(result.PendingPurchaseOrders, Is.Empty);
        Assert.That(result.PendingCustomerOrders, Is.Empty);
    }

    [Test]
    public async Task AggregatesSalesCashInventoryAndOrders_ForActiveBusiness()
    {
        var now = new DateTimeOffset(2025, 11, 27, 9, 30, 0, TimeSpan.Zero);
        await using var context = CreateContext(nameof(AggregatesSalesCashInventoryAndOrders_ForActiveBusiness));

        var businessId = 42;
        var store = new Store
        {
            BusinessId = businessId,
            Name = "Main Shop",
            IsActive = true,
            OwnerId = "owner"
        };

        var vendor = new Vendor
        {
            BusinessId = businessId,
            Name = "Mega Supplier"
        };

        var product = new Product
        {
            BusinessId = businessId,
            Name = "Sugar 1kg",
            SKU = "SUGAR-1KG",
            MinimumStockLevel = 10,
            ReorderQuantity = 25
        };

        var customer = new Customer
        {
            BusinessId = businessId,
            Store = store,
            StoreId = store.Id,
            FirstName = "Lerato",
            LastName = "M",
            Email = "lerato@example.com",
            IsActive = true
        };

        context.Stores.Add(store);
        context.Vendors.Add(vendor);
        context.Products.Add(product);
        context.Customers.Add(customer);

        await context.SaveChangesAsync();

        context.Sales.AddRange(
            new Sale
            {
                ShopId = store.Id,
                Shop = store,
                SaleDate = now.AddHours(-1),
                Status = SaleStatus.Completed,
                Total = 150m
            },
            new Sale
            {
                ShopId = store.Id,
                Shop = store,
                SaleDate = now.AddMinutes(-20),
                Status = SaleStatus.Completed,
                Total = 90m
            });

        context.Payments.AddRange(
            new Payment
            {
                ShopId = store.Id,
                Shop = store,
                Amount = 150m,
                PaymentDate = now.AddMinutes(-15),
                Status = PaymentStatus.Completed,
                SourceType = "Sale"
            },
            new Payment
            {
                ShopId = store.Id,
                Shop = store,
                Amount = 200m,
                PaymentDate = now.AddMinutes(-10),
                Status = PaymentStatus.Completed,
                SourceType = "PurchaseOrder"
            });

        context.StockAlerts.Add(new StockAlert
        {
            ShopId = store.Id,
            Shop = store,
            ProductId = product.Id,
            Product = product,
            CurrentStock = 4,
            MinimumStock = 10,
            IsAcknowledged = false
        });

        context.PurchaseOrders.Add(new PurchaseOrder
        {
            ShopId = store.Id,
            Shop = store,
            VendorId = vendor.Id,
            Vendor = vendor,
            PONumber = "PO-1001",
            Status = PurchaseOrderStatus.Pending,
            OrderDate = now.AddDays(-1),
            ExpectedDeliveryDate = now.AddHours(4),
            Total = 500m
        });

        context.Orders.Add(new Order
        {
            CustomerId = customer.Id,
            OrderStatus = OrderStatus.Pending,
            OrderTotal = 300m,
            Created = now.AddMinutes(-5)
        });

        await context.SaveChangesAsync();

        var service = CreateService(context, now);

        var result = await service.GetTodayAsync(businessId);

        Assert.That(result.Totals.SalesTotal, Is.EqualTo(240m));
        Assert.That(result.Totals.Transactions, Is.EqualTo(2));
        Assert.That(result.Totals.AverageTicket, Is.EqualTo(120m));
        Assert.That(result.Cash.CashIn, Is.EqualTo(150m));
        Assert.That(result.Cash.CashOut, Is.EqualTo(200m));
        Assert.That(result.LowStock.Count, Is.EqualTo(1));
        Assert.That(result.PendingPurchaseOrders.Count, Is.EqualTo(1));
        Assert.That(result.PendingCustomerOrders.Count, Is.EqualTo(1));
        Assert.That(result.Alerts, Is.Not.Empty);
    }

    private static ApplicationDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options, new TestBusinessContext());
    }

    private static OperationsTodayService CreateService(IApplicationDbContext context, DateTimeOffset utcNow)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var timeProvider = new FixedTimeProvider(utcNow);
        return new OperationsTodayService(
            context,
            cache,
            timeProvider,
            NullLogger<OperationsTodayService>.Instance);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}

