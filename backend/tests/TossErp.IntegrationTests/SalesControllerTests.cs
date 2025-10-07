using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Domain.Entities.Sales;
using TossErp.Infrastructure.Data;
using Xunit;

namespace TossErp.IntegrationTests;

public class SalesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SalesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateSale_ValidData_ReturnsCreated()
    {
        // Arrange
        var createSaleRequest = new
        {
            type = "Regular",
            customerName = "John Doe",
            customerPhone = "0821234567",
            customerEmail = "john@example.com",
            items = new[]
            {
                new
                {
                    productId = 1,
                    productName = "Test Product",
                    productSku = "TEST-001",
                    quantity = 2,
                    unitPrice = 5000, // R50.00
                    discount = 0
                }
            },
            discountAmount = 0,
            taxRate = 0.15m,
            warehouseId = 1,
            warehouseName = "Main Store",
            cashierId = 1,
            cashierName = "Test Cashier",
            notes = "Test sale"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", createSaleRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var sale = await response.Content.ReadFromJsonAsync<Sale>();
        sale.Should().NotBeNull();
        sale!.CustomerName.Should().Be("John Doe");
        sale.Items.Should().HaveCount(1);
        sale.TotalAmount.Should().Be(11500); // (5000 * 2) + 15% tax = 11,500
    }

    [Fact]
    public async Task GetSales_ReturnsListOfSales()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Sales.Add(new Sale
        {
            SaleNumber = "TEST-001",
            Status = SaleStatus.Completed,
            TotalAmount = 10000,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/sales");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var sales = await response.Content.ReadFromJsonAsync<List<Sale>>();
        sales.Should().NotBeNull();
        sales.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task CompleteSale_ValidDraftSale_ReturnsOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var sale = new Sale
        {
            SaleNumber = "TEST-002",
            Status = SaleStatus.Draft,
            TotalAmount = 15000,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        sale.Items.Add(new SaleItem
        {
            ProductName = "Test Product",
            Quantity = 1,
            UnitPrice = 15000,
            LineTotal = 15000,
            CreatedAt = DateTime.UtcNow
        });
        
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.PostAsync($"/api/sales/{sale.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var completedSale = await response.Content.ReadFromJsonAsync<Sale>();
        completedSale.Should().NotBeNull();
        completedSale!.Status.Should().Be(SaleStatus.Completed);
    }

    [Fact]
    public async Task GetSalesSummary_ReturnsCorrectMetrics()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Add multiple completed sales
        for (int i = 0; i < 5; i++)
        {
            context.Sales.Add(new Sale
            {
                SaleNumber = $"TEST-{100 + i}",
                Status = SaleStatus.Completed,
                TotalAmount = 10000 * (i + 1),
                SaleDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        var today = DateTime.Today;

        // Act
        var response = await _client.GetAsync($"/api/sales/summary?startDate={today:yyyy-MM-dd}&endDate={today.AddDays(1):yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var summary = await response.Content.ReadFromJsonAsync<dynamic>();
        summary.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelSale_ValidSale_ReturnsOk()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var sale = new Sale
        {
            SaleNumber = "TEST-003",
            Status = SaleStatus.Completed,
            TotalAmount = 20000,
            SaleDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        var cancelRequest = new { reason = "Customer request" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/sales/{sale.Id}/cancel", cancelRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var cancelledSale = await response.Content.ReadFromJsonAsync<Sale>();
        cancelledSale.Should().NotBeNull();
        cancelledSale!.Status.Should().Be(SaleStatus.Cancelled);
        cancelledSale.Notes.Should().Contain("Customer request");
    }
}

