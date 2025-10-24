using Toss.Application.GroupBuying.Commands.CreatePool;
using Toss.Application.GroupBuying.Commands.JoinPool;
using Toss.Domain.Entities;
using Toss.Domain.Entities.GroupBuying;
using Toss.Domain.Entities.Inventory;
using Toss.Domain.Entities.Suppliers;
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
        var initiatorShop = new Shop
        {
            Name = "Initiator Shop",
            OwnerId = userId,
            Email = "initiator@shop.com"
        };
        await AddAsync(initiatorShop);

        // Create joining shop
        var joiningShop = new Shop
        {
            Name = "Joining Shop",
            OwnerId = userId,
            Email = "joining@shop.com"
        };
        await AddAsync(joiningShop);

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

        // Create pool
        var createCommand = new CreatePoolCommand
        {
            ShopId = initiatorShop.Id,
            ProductId = product.Id,
            SupplierId = supplier.Id,
            TargetQuantity = 100,
            UnitPrice = 80,
            InitialQuantity = 30,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7)
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
        pool!.CurrentQuantity.ShouldBe(55); // 30 + 25

        var participationCount = await CountAsync<PoolParticipation>();
        participationCount.ShouldBe(2); // Initiator + Joiner
    }

    [Test]
    public async Task ShouldNotAllowJoiningClosedPool()
    {
        var userId = await RunAsDefaultUserAsync();

        var initiatorShop = new Shop
        {
            Name = "Initiator Shop",
            OwnerId = userId,
            Email = "initiator@shop.com"
        };
        await AddAsync(initiatorShop);

        var joiningShop = new Shop
        {
            Name = "Joining Shop",
            OwnerId = userId,
            Email = "joining@shop.com"
        };
        await AddAsync(joiningShop);

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

        // Create a closed pool
        var pool = new GroupBuyPool
        {
            InitiatorShopId = initiatorShop.Id,
            ProductId = product.Id,
            SupplierId = supplier.Id,
            TargetQuantity = 100,
            UnitPrice = 80,
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

        await Should.ThrowAsync<ValidationException>(() => SendAsync(joinCommand));
    }

    [Test]
    public async Task ShouldNotAllowDuplicateParticipation()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            ContactEmail = "test@shop.com"
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

        var createCommand = new CreatePoolCommand
        {
            ShopId = shop.Id,
            ProductId = product.Id,
            SupplierId = supplier.Id,
            TargetQuantity = 100,
            UnitPrice = 80,
            InitialQuantity = 10,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7)
        };

        var poolId = await SendAsync(createCommand);

        // Try to join own pool
        var joinCommand = new JoinPoolCommand
        {
            GroupBuyPoolId = poolId,
            ShopId = shop.Id, // Same shop trying to join again
            QuantityCommitted = 10
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(joinCommand));
    }
}

