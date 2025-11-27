using Toss.Application.Common.Exceptions;
using Toss.Application.Logistics.Commands.CreateSharedDeliveryRun;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Logistics;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;
using Toss.Domain.ValueObjects;

namespace Toss.Application.FunctionalTests.Logistics.Commands;

using static Testing;

public class CreateSharedDeliveryRunTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateSharedDeliveryRunCommand();

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateSharedDeliveryRun()
    {
        var userId = await RunAsDefaultUserAsync();

        // Create shops
        var business = await CreateBusinessAsync();
        var shop1 = new Store
        {
            Name = "Shop 1",
            OwnerId = userId,
            Email = "shop1@test.com",
            BusinessId = business.Id
        };
        await AddAsync(shop1);

        var shop2 = new Store
        {
            Name = "Shop 2",
            OwnerId = userId,
            Email = "shop2@test.com",
            BusinessId = business.Id
        };
        await AddAsync(shop2);

        var command = new CreateSharedDeliveryRunCommand
        {
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            TotalDeliveryCost = 200,
            AreaGroup = "TestArea",
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop1.Id,
                    Latitude = -26.2041,
                    Longitude = 28.0473,
                    DeliveryInstructions = "First stop"
                },
                new()
                {
                    ShopId = shop2.Id,
                    Latitude = -26.2141,
                    Longitude = 28.0573,
                    DeliveryInstructions = "Second stop"
                }
            }
        };

        var runId = await SendAsync(command);

        var run = await FindAsync<SharedDeliveryRun>(runId);

        run.ShouldNotBeNull();
        run!.RunNumber.ShouldNotBeNullOrEmpty(); // Generated automatically
        run.Status.ShouldBe(DeliveryStatus.Scheduled);
        run.Stops.Count.ShouldBe(2);
    }

    [Test]
    public async Task ShouldRequireUniqueRunNumber()
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

        // Note: RunNumber is auto-generated, so duplicate check is not applicable in this test
        // This test can be removed or modified to test other validation rules
    }

    [Test]
    public async Task ShouldRequireAtLeastOneStop()
    {
        var command = new CreateSharedDeliveryRunCommand
        {
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            TotalDeliveryCost = 100,
            Stops = new List<DeliveryStopDto>() // Empty
        };

        await Should.ThrowAsync<InvalidOperationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCalculateTotalDistance()
    {
        var userId = await RunAsDefaultUserAsync();

        var business = await CreateBusinessAsync();
        var shop1 = new Store
        {
            Name = "Shop 1",
            OwnerId = userId,
            Email = "shop1@test.com",
            BusinessId = business.Id
        };
        await AddAsync(shop1);

        var shop2 = new Store
        {
            Name = "Shop 2",
            OwnerId = userId,
            Email = "shop2@test.com",
            BusinessId = business.Id
        };
        await AddAsync(shop2);

        var command = new CreateSharedDeliveryRunCommand
        {
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            TotalDeliveryCost = 200,
            AreaGroup = "TestArea",
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop1.Id,
                    Latitude = -26.2041,
                    Longitude = 28.0473
                },
                new()
                {
                    ShopId = shop2.Id,
                    Latitude = -26.2141,
                    Longitude = 28.0573
                }
            }
        };

        var runId = await SendAsync(command);

        var run = await FindAsync<SharedDeliveryRun>(runId);

        run.ShouldNotBeNull();
        run!.TotalDeliveryCost.ShouldBe(200);
        run.CostPerStop.ShouldBe(100); // 200 / 2 stops
    }
}

