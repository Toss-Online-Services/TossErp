using Toss.Application.Common.Exceptions;
using Toss.Application.GroupBuying.Commands.CreatePool;
using Toss.Domain.Entities;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Suppliers;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.GroupBuying.Commands;

using static Testing;

public class CreatePoolTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreatePoolCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldRequireValidShop()
    {
        var command = new CreatePoolCommand
        {
            ShopId = 999,
            ProductId = 1,
            SupplierId = 1,
            TargetQuantity = 100,
            UnitPrice = 10,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7)
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateGroupBuyPool()
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
            Name = "Bulk Product",
            SKU = "BULK-001",
            BasePrice = 100
        };
        await AddAsync(product);

        // Create supplier
        var supplier = new Supplier
        {
            Name = "Test Supplier",
            ContactEmail = "supplier@test.com"
        };
        await AddAsync(supplier);

        var command = new CreatePoolCommand
        {
            ShopId = shop.Id,
            ProductId = product.Id,
            SupplierId = supplier.Id,
            TargetQuantity = 100,
            UnitPrice = 80, // Bulk discount price
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            Description = "Bulk purchase for better pricing"
        };

        var poolId = await SendAsync(command);

        var pool = await FindAsync<GroupBuyPool>(poolId);

        pool.ShouldNotBeNull();
        pool!.InitiatorShopId.ShouldBe(shop.Id);
        pool.ProductId.ShouldBe(product.Id);
        pool.SupplierId.ShouldBe(supplier.Id);
        pool.TargetQuantity.ShouldBe(100);
        pool.UnitPrice.ShouldBe(80);
        pool.Status.ShouldBe(PoolStatus.Open);
    }

    [Test]
    public async Task ShouldCreateInitialParticipationForCreator()
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
            Name = "Bulk Product",
            SKU = "BULK-001",
            BasePrice = 100
        };
        await AddAsync(product);

        var supplier = new Supplier
        {
            Name = "Test Supplier",
            ContactEmail = "supplier@test.com"
        };
        await AddAsync(supplier);

        var command = new CreatePoolCommand
        {
            ShopId = shop.Id,
            ProductId = product.Id,
            SupplierId = supplier.Id,
            TargetQuantity = 100,
            UnitPrice = 80,
            InitialQuantity = 20, // Creator wants 20 units
            CloseDate = DateTimeOffset.UtcNow.AddDays(7)
        };

        var poolId = await SendAsync(command);

        var pool = await FindAsync<GroupBuyPool>(poolId);
        pool.ShouldNotBeNull();
        pool!.CurrentQuantity.ShouldBe(20);

        var participationCount = await CountAsync<PoolParticipation>();
        participationCount.ShouldBe(1);
    }
}

