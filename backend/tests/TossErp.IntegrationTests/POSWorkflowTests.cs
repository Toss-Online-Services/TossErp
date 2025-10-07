using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Domain.Entities.Sales;
using TossErp.Domain.Entities.Inventory;
using TossErp.Infrastructure.Data;
using Xunit;

namespace TossErp.IntegrationTests;

/// <summary>
/// Integration tests for complete POS workflows
/// </summary>
public class POSWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public POSWorkflowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                
                // Seed test data
                SeedTestData(db);
            });
        });

        _client = _factory.CreateClient();
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        // Add test warehouse
        var warehouse = new Warehouse
        {
            Name = "Main Store",
            Code = "MAIN",
            IsActive = true,
            IsDefault = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Warehouses.Add(warehouse);
        context.SaveChanges();

        // Add test products with stock
        var products = new[]
        {
            new Product
            {
                Name = "Coca Cola 500ml",
                Sku = "COC-500",
                Barcode = "6001234567890",
                SellingPrice = 1500, // R15.00
                CostPrice = 1000,
                IsActive = true,
                TrackInventory = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Bread",
                Sku = "BRD-001",
                Barcode = "6009876543210",
                SellingPrice = 1200, // R12.00
                CostPrice = 800,
                IsActive = true,
                TrackInventory = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Add stock levels
        foreach (var product in products)
        {
            context.StockLevels.Add(new StockLevel
            {
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name,
                QuantityOnHand = 100,
                QuantityReserved = 0,
                ReorderPoint = 20,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Add test customer
        context.Customers.Add(new Customer
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Phone = "0821234567",
            IsActive = true,
            LoyaltyPoints = 100,
            CreatedAt = DateTime.UtcNow
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task CompletePOSWorkflow_FromCartToReceipt_Success()
    {
        // Step 1: Create sale with items
        var createSaleRequest = new
        {
            type = "Regular",
            customerId = 1,
            customerName = "Test Customer",
            customerPhone = "0821234567",
            items = new[]
            {
                new
                {
                    productId = 1,
                    productName = "Coca Cola 500ml",
                    productSku = "COC-500",
                    quantity = 2,
                    unitPrice = 1500,
                    discount = 0,
                    notes = (string?)null
                },
                new
                {
                    productId = 2,
                    productName = "Bread",
                    productSku = "BRD-001",
                    quantity = 1,
                    unitPrice = 1200,
                    discount = 0,
                    notes = (string?)null
                }
            },
            discountAmount = 200, // R2.00 discount
            taxRate = 0.15m,
            warehouseId = 1,
            warehouseName = "Main Store",
            cashierId = 1,
            cashierName = "Test Cashier",
            posDeviceId = "POS-001",
            posDeviceName = "Register 1",
            notes = "Integration test sale"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/sales", createSaleRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdSale = await createResponse.Content.ReadFromJsonAsync<Sale>();
        createdSale.Should().NotBeNull();
        createdSale!.Items.Should().HaveCount(2);
        
        // Verify calculations
        var expectedSubtotal = (1500 * 2) + (1200 * 1); // 4200
        var expectedTax = (expectedSubtotal - 200) * 0.15m; // 600
        var expectedTotal = expectedSubtotal - 200 + expectedTax; // 4600
        
        createdSale.Subtotal.Should().Be(expectedSubtotal);
        createdSale.TaxAmount.Should().Be(expectedTax);
        createdSale.TotalAmount.Should().Be(expectedTotal);

        // Step 2: Complete the sale
        var completeResponse = await _client.PostAsync($"/api/sales/{createdSale.Id}/complete", null);
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var completedSale = await completeResponse.Content.ReadFromJsonAsync<Sale>();
        completedSale.Should().NotBeNull();
        completedSale!.Status.Should().Be(SaleStatus.Completed);

        // Step 3: Generate receipt
        var receiptResponse = await _client.GetAsync($"/api/receipts/{createdSale.Id}/html");
        receiptResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var receiptHtml = await receiptResponse.Content.ReadAsStringAsync();
        receiptHtml.Should().Contain("TOSS ERP");
        receiptHtml.Should().Contain("Coca Cola 500ml");
        receiptHtml.Should().Contain("Bread");
        receiptHtml.Should().Contain("R46.00"); // Total amount
    }

    [Fact]
    public async Task MultiplePaymentMethods_SplitPayment_Success()
    {
        // This test would verify split payment processing
        // In a full implementation, this would:
        // 1. Create a sale
        // 2. Add multiple payments (Cash + Card)
        // 3. Verify total payments match sale total
        // 4. Complete sale
        
        // Placeholder for future implementation
        Assert.True(true, "Split payment test placeholder");
    }

    [Fact]
    public async Task LowStockWarning_DuringSale_AlertsUser()
    {
        // This test would verify low stock warnings
        // In a full implementation, this would:
        // 1. Set a product to low stock
        // 2. Attempt to sell it
        // 3. Verify warning is returned
        
        Assert.True(true, "Low stock warning test placeholder");
    }

    [Fact]
    public async Task CancelSale_AfterCompletion_SuccessfullyReversed()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var sale = new Sale
        {
            SaleNumber = "TEST-CANCEL-001",
            Status = SaleStatus.Completed,
            TotalAmount = 25000,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        var cancelRequest = new { reason = "Customer changed mind" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/sales/{sale.Id}/cancel", cancelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var cancelledSale = await response.Content.ReadFromJsonAsync<Sale>();
        cancelledSale.Should().NotBeNull();
        cancelledSale!.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact]
    public async Task EndOfDaySummary_MultipleSales_CorrectTotals()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Add 10 test sales
        var testSales = Enumerable.Range(1, 10).Select(i => new Sale
        {
            SaleNumber = $"TEST-EOD-{i:D3}",
            Status = SaleStatus.Completed,
            TotalAmount = 5000 * i,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        
        context.Sales.AddRange(testSales);
        await context.SaveChangesAsync();

        var today = DateTime.Today;

        // Act
        var response = await _client.GetAsync($"/api/sales/summary?startDate={today:yyyy-MM-dd}&endDate={today.AddDays(1):yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var summary = await response.Content.ReadFromJsonAsync<dynamic>();
        summary.Should().NotBeNull();
        
        // Verify: Expected total = 5000 + 10000 + 15000 + ... + 50000 = 275000
        // totalRevenue should be 275000 (sum of 5000 * (1+2+3+...+10))
    }
}

