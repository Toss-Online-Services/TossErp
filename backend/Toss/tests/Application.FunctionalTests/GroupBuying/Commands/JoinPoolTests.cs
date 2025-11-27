using Toss.Application.GroupBuying.Commands.CreatePool;
using Toss.Application.GroupBuying.Commands.JoinPool;
using Toss.Domain.Entities;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Catalog;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;

namespace Toss.Application.FunctionalTests.GroupBuying.Commands;

using static Testing;

public class JoinPoolTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidPool()
    {
        var command = new JoinPoolCommand
        {
            GroupBuyPoolId = 999,
            ShopId = 1,
            QuantityCommitted = 10
        };

        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldAllowShopToJoinPool()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create initiator shop
        var initiatorBusiness = await CreateBusinessAsync();
        var initiatorShop = new Store
        {
            Name = "Initiator Shop",
            OwnerId = userId,
            Email = "initiator@shop.com",
            BusinessId = initiatorBusiness.Id
        };
        await AddAsync(initiatorShop);

        // Create joining shop
        var joiningBusiness = await CreateBusinessAsync();
        var joiningShop = new Store
        {
            Name = "Joining Shop",
            OwnerId = userId,
            Email = "joining@shop.com",
            BusinessId = joiningBusiness.Id
        };
        await AddAsync(joiningShop);

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

        // Create pool
        var createCommand = new CreatePoolCommand
        {
            Title = "Bulk Purchase Test",
            InitiatorShopId = initiatorShop.Id,
            ProductId = product.Id,
            VendorId = supplier.Id,
            MinimumQuantity = 100,
            UnitPrice = 100,
            BulkDiscountPercentage = 20,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            EstimatedShippingCost = 500
        };

        var poolId = await SendAsync(createCommand);

        // Join pool
        var joinCommand = new JoinPoolCommand
        {
            GroupBuyPoolId = poolId,
            ShopId = joiningShop.Id,
            QuantityCommitted = 25
        };

        await SendAsync(joinCommand);

        var pool = await FindAsync<GroupBuyPool>(poolId);
        pool.ShouldNotBeNull();
        pool!.CurrentQuantity.ShouldBe(25); // Only joiner (initiator doesn't auto-join in this flow)

        var participationCount = await CountAsync<PoolParticipation>();
        participationCount.ShouldBe(1); // Just the joiner
    }

    [Test]
    public async Task ShouldNotAllowJoiningClosedPool()
    {
        var userId = await RunAsDefaultUserAsync();

        var initiatorBusiness = await CreateBusinessAsync();
        var initiatorShop = new Store
        {
            Name = "Initiator Shop",
            OwnerId = userId,
            Email = "initiator@shop.com",
            BusinessId = initiatorBusiness.Id
        };
        await AddAsync(initiatorShop);

        var joiningBusiness = await CreateBusinessAsync();
        var joiningShop = new Store
        {
            Name = "Joining Shop",
            OwnerId = userId,
            Email = "joining@shop.com",
            BusinessId = joiningBusiness.Id
        };
        await AddAsync(joiningShop);

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

        // Create a closed pool
        var pool = new GroupBuyPool
        {
            PoolNumber = "POOL-TEST-001",
            Title = "Closed Pool",
            InitiatorShopId = initiatorShop.Id,
            ProductId = product.Id,
            VendorId = supplier.Id,
            MinimumQuantity = 100,
            UnitPrice = 100,
            FinalUnitPrice = 80,
            OpenDate = DateTimeOffset.UtcNow.AddDays(-1),
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            Status = PoolStatus.Confirmed // Already confirmed/closed
        };
        await AddAsync(pool);

        var joinCommand = new JoinPoolCommand
        {
            GroupBuyPoolId = pool.Id,
            ShopId = joiningShop.Id,
            QuantityCommitted = 10
        };

        await Should.ThrowAsync<InvalidOperationException>(() => SendAsync(joinCommand));
    }

    [Test]
    public async Task ShouldNotAllowDuplicateParticipation()
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

        var createCommand = new CreatePoolCommand
        {
            Title = "Test Pool",
            InitiatorShopId = shop.Id,
            ProductId = product.Id,
            VendorId = supplier.Id,
            MinimumQuantity = 100,
            UnitPrice = 100,
            BulkDiscountPercentage = 20,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7),
            EstimatedShippingCost = 500
        };

        var poolId = await SendAsync(createCommand);

        // Try to join own pool
        var joinCommand = new JoinPoolCommand
        {
            GroupBuyPoolId = poolId,
            ShopId = shop.Id, // Same shop trying to join again
            QuantityCommitted = 10
        };

        await Should.ThrowAsync<InvalidOperationException>(() => SendAsync(joinCommand));
    }
}

