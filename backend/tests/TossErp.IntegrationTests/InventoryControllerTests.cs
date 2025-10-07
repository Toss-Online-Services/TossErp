using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Domain.Entities.Inventory;
using TossErp.Infrastructure.Data;
using Xunit;

namespace TossErp.IntegrationTests;

public class InventoryControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public InventoryControllerTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase($"InventoryTestDb_{Guid.NewGuid()}");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task StockAdjustment_IncreasesStockLevel()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var product = new Product
        {
            Name = "Test Product",
            Sku = "TEST-SKU",
            SellingPrice = 1000,
            CostPrice = 500,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        var stockLevel = new StockLevel
        {
            ProductId = product.Id,
            WarehouseId = 1,
            WarehouseName = "Main Warehouse",
            QuantityOnHand = 50,
            QuantityReserved = 0,
            CreatedAt = DateTime.UtcNow
        };
        context.StockLevels.Add(stockLevel);
        await context.SaveChangesAsync();

        var adjustRequest = new
        {
            quantity = 20,
            reason = "Stock count adjustment"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/inventory/stock-levels/{stockLevel.Id}/adjust", adjustRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updatedStock = await response.Content.ReadFromJsonAsync<StockLevel>();
        updatedStock.Should().NotBeNull();
        updatedStock!.QuantityOnHand.Should().Be(70); // 50 + 20
    }

    [Fact]
    public async Task StockTransfer_BetweenWarehouses_Success()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Create warehouses
        var warehouse1 = new Warehouse { Name = "Warehouse 1", Code = "WH1", IsActive = true, CreatedAt = DateTime.UtcNow };
        var warehouse2 = new Warehouse { Name = "Warehouse 2", Code = "WH2", IsActive = true, CreatedAt = DateTime.UtcNow };
        context.Warehouses.AddRange(warehouse1, warehouse2);
        await context.SaveChangesAsync();
        
        // Create product and stock
        var product = new Product
        {
            Name = "Transfer Test Product",
            Sku = "TRANSFER-001",
            SellingPrice = 2000,
            CostPrice = 1000,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        var stockLevel1 = new StockLevel
        {
            ProductId = product.Id,
            WarehouseId = warehouse1.Id,
            WarehouseName = warehouse1.Name,
            QuantityOnHand = 100,
            QuantityReserved = 0,
            CreatedAt = DateTime.UtcNow
        };
        context.StockLevels.Add(stockLevel1);
        await context.SaveChangesAsync();

        var transferRequest = new
        {
            productId = product.Id,
            fromWarehouseId = warehouse1.Id,
            toWarehouseId = warehouse2.Id,
            quantity = 30,
            notes = "Stock transfer test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/inventory/stock-movements/transfer", transferRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify stock levels updated
        var sourceStock = await context.StockLevels
            .FirstOrDefaultAsync(s => s.ProductId == product.Id && s.WarehouseId == warehouse1.Id);
        sourceStock.Should().NotBeNull();
        sourceStock!.QuantityOnHand.Should().Be(70); // 100 - 30
        
        var destStock = await context.StockLevels
            .FirstOrDefaultAsync(s => s.ProductId == product.Id && s.WarehouseId == warehouse2.Id);
        destStock.Should().NotBeNull();
        destStock!.QuantityOnHand.Should().Be(30);
    }

    [Fact]
    public async Task LowStockDetection_TriggersAlert()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var product = new Product
        {
            Name = "Low Stock Product",
            Sku = "LOW-001",
            SellingPrice = 500,
            CostPrice = 300,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        var stockLevel = new StockLevel
        {
            ProductId = product.Id,
            WarehouseId = 1,
            WarehouseName = "Test Warehouse",
            QuantityOnHand = 5, // Below reorder point
            QuantityReserved = 0,
            ReorderPoint = 10,
            ReorderQuantity = 50,
            CreatedAt = DateTime.UtcNow
        };
        context.StockLevels.Add(stockLevel);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/inventory/stock-levels?lowStockOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var lowStockItems = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        lowStockItems.Should().NotBeNull();
        lowStockItems.Should().HaveCountGreaterThan(0);
    }
}

