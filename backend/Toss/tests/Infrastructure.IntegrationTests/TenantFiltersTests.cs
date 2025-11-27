using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Toss.Domain.Entities.Sales;
using Toss.Domain.Entities.Stores;
using Toss.Infrastructure.Data;

namespace Toss.Infrastructure.IntegrationTests;

[TestFixture]
public class TenantFiltersTests
{
    [Test]
    public async Task QueryFilters_ScopeEntities_ToCurrentBusiness()
    {
        await using var context = CreateContext(out var businessContext);

        var storeA = CreateStore(1, "owner-a");
        var storeB = CreateStore(2, "owner-b");

        context.Stores.AddRange(storeA, storeB);
        await context.SaveChangesAsync();

        var saleA = CreateSale(storeA.Id, "SALE-1000");
        var saleB = CreateSale(storeB.Id, "SALE-2000");

        context.Sales.AddRange(saleA, saleB);
        await context.SaveChangesAsync();

        businessContext.SetBusiness(1, "BIZ-1", "Biz One");
        var businessOneStores = await context.Stores.ToListAsync();
        var businessOneSales = await context.Sales.ToListAsync();

        Assert.That(businessOneStores, Has.Count.EqualTo(1));
        Assert.That(businessOneStores.Single().BusinessId, Is.EqualTo(1));
        Assert.That(businessOneSales, Has.Count.EqualTo(1));
        Assert.That(businessOneSales.Single().ShopId, Is.EqualTo(storeA.Id));

        businessContext.SetBusiness(2, "BIZ-2", "Biz Two");
        var businessTwoStores = await context.Stores.ToListAsync();
        var businessTwoSales = await context.Sales.ToListAsync();

        Assert.That(businessTwoStores, Has.Count.EqualTo(1));
        Assert.That(businessTwoStores.Single().BusinessId, Is.EqualTo(2));
        Assert.That(businessTwoSales, Has.Count.EqualTo(1));
        Assert.That(businessTwoSales.Single().ShopId, Is.EqualTo(storeB.Id));
    }

    private static Store CreateStore(int businessId, string ownerId) =>
        new()
        {
            BusinessId = businessId,
            Name = $"Store-{businessId}",
            OwnerId = ownerId,
            Currency = "ZAR",
            TaxRate = 15m,
            Language = "en",
            Timezone = "Africa/Johannesburg",
            WhatsAppAlertsEnabled = true,
            GroupBuyingEnabled = true,
            AIAssistantEnabled = true
        };

    private static Sale CreateSale(int shopId, string saleNumber) =>
        new()
        {
            ShopId = shopId,
            SaleNumber = saleNumber,
            SaleDate = DateTimeOffset.UtcNow,
            PaymentMethod = Domain.Enums.PaymentType.Cash,
            Status = Domain.Enums.SaleStatus.Completed
        };

    private static ApplicationDbContext CreateContext(out TestBusinessContext businessContext)
    {
        businessContext = new TestBusinessContext();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, businessContext);
    }
}

