using Toss.Application.Common.Exceptions;
using Toss.Application.GroupBuying.Commands.CreatePool;
using Toss.Domain.Entities;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
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
            Title = "Test Pool",
            InitiatorShopId = 999,
            ProductId = 1,
            VendorId = 1,
            MinimumQuantity = 100,
            UnitPrice = 100,
            BulkDiscountPercentage = 20,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            EstimatedShippingCost = 500
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateGroupBuyPool()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create shop
        var shop = new Store
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
        var supplier = new Vendor
        {
            Name = "Test Vendor",
            Email = "supplier@test.com"
        };
        await AddAsync(supplier);

        var command = new CreatePoolCommand
        {
            Title = "Bulk Purchase Pool",
            Description = "Bulk purchase for better pricing",
            InitiatorShopId = shop.Id,
            ProductId = product.Id,
            VendorId = supplier.Id,
            MinimumQuantity = 100,
            UnitPrice = 100,
            BulkDiscountPercentage = 20, // 20% discount
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            EstimatedShippingCost = 500
        };

        var poolId = await SendAsync(command);

        var pool = await FindAsync<GroupBuyPool>(poolId);

        pool.ShouldNotBeNull();
        pool!.InitiatorShopId.ShouldBe(shop.Id);
        pool.ProductId.ShouldBe(product.Id);
        pool.VendorId.ShouldBe(supplier.Id);
        pool.MinimumQuantity.ShouldBe(100);
        pool.UnitPrice.ShouldBe(100);
        pool.FinalUnitPrice.ShouldBe(80); // After 20% discount
        pool.Status.ShouldBe(PoolStatus.Open);
    }

    [Test]
    public async Task ShouldCreateInitialParticipationForCreator()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop = new Store
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

        var supplier = new Vendor
        {
            Name = "Test Vendor",
            Email = "supplier@test.com"
        };
        await AddAsync(supplier);

        var command = new CreatePoolCommand
        {
            Title = "Test Pool with Initial Participation",
            InitiatorShopId = shop.Id,
            ProductId = product.Id,
            VendorId = supplier.Id,
            MinimumQuantity = 100,
            UnitPrice = 100,
            BulkDiscountPercentage = 20,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            EstimatedShippingCost = 500
        };

        var poolId = await SendAsync(command);

        var pool = await FindAsync<GroupBuyPool>(poolId);
        pool.ShouldNotBeNull();
        pool!.CurrentQuantity.ShouldBe(0); // No initial participation in this flow

        // No automatic participation in CreatePoolCommand
        var participationCount = await CountAsync<PoolParticipation>();
        participationCount.ShouldBe(0);
    }
}

