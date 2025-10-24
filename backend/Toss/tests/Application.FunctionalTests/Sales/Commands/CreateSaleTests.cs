using Toss.Application.Common.Exceptions;
using Toss.Application.Sales.Commands.CreateSale;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Sales.Commands;

using static Testing;

public class CreateSaleTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateSaleCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireValidShop()
    {
        var command = new CreateSaleCommand
        {
            ShopId = 999,
            Items = new List<SaleItemDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 10 }
            }
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireValidProduct()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create shop
        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com"
        };
        await AddAsync(shop);

        var command = new CreateSaleCommand
        {
            ShopId = shop.Id,
            Items = new List<SaleItemDto>
            {
                new() { ProductId = 999, Quantity = 1, UnitPrice = 10 }
            }
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateSaleAndDeductStock()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create shop
        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com"
        };
        await AddAsync(shop);

        // Create product
        var product = new Product
        {
            Name = "Test Product",
            SKU = "TEST-001",
            BasePrice = 10
        };
        await AddAsync(product);

        // Create stock level
        var stockLevel = new StockLevel
        {
            ProductId = product.Id,
            ShopId = shop.Id,
            CurrentStock = 100,
            ReorderPoint = 10,
            ReorderQuantity = 50
        };
        await AddAsync(stockLevel);

        var command = new CreateSaleCommand
        {
            ShopId = shop.Id,
            Items = new List<SaleItemDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = 5,
                    UnitPrice = 10
                }
            }
        };

        var saleId = await SendAsync(command);

        var sale = await FindAsync<Sale>(saleId);
        sale.ShouldNotBeNull();
        sale!.ShopId.ShouldBe(shop.Id);
        sale.Status.ShouldBe(SaleStatus.Completed);
        sale.Total.ShouldBe(50); // 5 * 10
        sale.Items.Count.ShouldBe(1);
        sale.Items.First().Quantity.ShouldBe(5);

        // Verify stock was deducted
        var updatedStock = await FindAsync<StockLevel>(stockLevel.Id);
        updatedStock.ShouldNotBeNull();
        updatedStock!.CurrentStock.ShouldBe(95); // 100 - 5
    }

    [Test]
    public async Task ShouldCreateLowStockAlertWhenBelowReorderPoint()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com"
        };
        await AddAsync(shop);

        var product = new Product
        {
            Name = "Test Product",
            SKU = "TEST-001",
            BasePrice = 10
        };
        await AddAsync(product);

        var stockLevel = new StockLevel
        {
            ProductId = product.Id,
            ShopId = shop.Id,
            CurrentStock = 15, // Just above reorder point
            ReorderPoint = 10,
            ReorderQuantity = 50
        };
        await AddAsync(stockLevel);

        var command = new CreateSaleCommand
        {
            ShopId = shop.Id,
            Items = new List<SaleItemDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = 10, // This will bring stock to 5, below reorder point
                    UnitPrice = 10
                }
            }
        };

        var saleId = await SendAsync(command);

        var sale = await FindAsync<Sale>(saleId);
        sale.ShouldNotBeNull();

        // Verify stock alert was created
        var alertCount = await CountAsync<StockAlert>();
        alertCount.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task ShouldHandleMultipleItemsInSale()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com"
        };
        await AddAsync(shop);

        var product1 = new Product { Name = "Product 1", SKU = "PROD-001", BasePrice = 10 };
        var product2 = new Product { Name = "Product 2", SKU = "PROD-002", BasePrice = 20 };
        await AddAsync(product1);
        await AddAsync(product2);

        var stock1 = new StockLevel { ProductId = product1.Id, ShopId = shop.Id, CurrentStock = 100 };
        var stock2 = new StockLevel { ProductId = product2.Id, ShopId = shop.Id, CurrentStock = 50 };
        await AddAsync(stock1);
        await AddAsync(stock2);

        var command = new CreateSaleCommand
        {
            ShopId = shop.Id,
            Items = new List<SaleItemDto>
            {
                new() { ProductId = product1.Id, Quantity = 3, UnitPrice = 10 },
                new() { ProductId = product2.Id, Quantity = 2, UnitPrice = 20 }
            }
        };

        var saleId = await SendAsync(command);

        var sale = await FindAsync<Sale>(saleId);
        sale.ShouldNotBeNull();
        sale!.Total.ShouldBe(70); // (3 * 10) + (2 * 20)
        sale.Items.Count.ShouldBe(2);
    }
}

