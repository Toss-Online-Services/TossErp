using Toss.Application.Sales.Commands.CreateSale;
using Toss.Application.Sales.Commands.VoidSale;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.Sales.Commands;

using static Testing;

public class VoidSaleTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidSaleId()
    {
        var command = new VoidSaleCommand
        {
            SaleId = 999,
            Reason = "Test void"
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldVoidSaleAndRestoreStock()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create shop
        var business = await CreateBusinessAsync();
        var shop = new Store
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com",
            BusinessId = business.Id
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
            CurrentStock = 100
        };
        await AddAsync(stockLevel);

        // Create a sale first
        var createCommand = new CreateSaleCommand
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

        var saleId = await SendAsync(createCommand);

        // Verify stock was deducted
        var stockAfterSale = await FindAsync<StockLevel>(stockLevel.Id);
        stockAfterSale!.CurrentStock.ShouldBe(95);

        // Now void the sale
        var voidCommand = new VoidSaleCommand
        {
            SaleId = saleId,
            Reason = "Customer returned items"
        };

        await SendAsync(voidCommand);

        // Verify sale status changed
        var voidedSale = await FindAsync<Sale>(saleId);
        voidedSale.ShouldNotBeNull();
        voidedSale!.Status.ShouldBe(SaleStatus.Voided);
        voidedSale.VoidReason.ShouldBe("Customer returned items");
        voidedSale.VoidedAt.ShouldNotBeNull();

        // Verify stock was restored
        var stockAfterVoid = await FindAsync<StockLevel>(stockLevel.Id);
        stockAfterVoid!.CurrentStock.ShouldBe(100); // Back to original
    }

    [Test]
    public async Task ShouldNotAllowVoidingAlreadyVoidedSale()
    {
        var userId = await RunAsDefaultUserAsync();

        var business = await CreateBusinessAsync();
        var shop = new Store
        {
            Name = "Test Shop",
            OwnerId = userId,
            Email = "test@shop.com",
            BusinessId = business.Id
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
            CurrentStock = 100
        };
        await AddAsync(stockLevel);

        // Create and void a sale
        var createCommand = new CreateSaleCommand
        {
            ShopId = shop.Id,
            Items = new List<SaleItemDto>
            {
                new() { ProductId = product.Id, Quantity = 5, UnitPrice = 10 }
            }
        };

        var saleId = await SendAsync(createCommand);

        var voidCommand = new VoidSaleCommand
        {
            SaleId = saleId,
            Reason = "First void"
        };

        await SendAsync(voidCommand);

        // Try to void again
        var secondVoidCommand = new VoidSaleCommand
        {
            SaleId = saleId,
            Reason = "Second void"
        };

        await Should.ThrowAsync<InvalidOperationException>(() => SendAsync(secondVoidCommand));
    }
}

