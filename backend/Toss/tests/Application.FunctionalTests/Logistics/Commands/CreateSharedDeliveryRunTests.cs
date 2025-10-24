using Toss.Application.Common.Exceptions;
using Toss.Application.Logistics.Commands.CreateSharedDeliveryRun;
using Toss.Domain.Entities;
using Toss.Domain.Entities.Logistics;
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
        var shop1 = new Shop
        {
            Name = "Shop 1",
            OwnerId = userId,
            ContactEmail = "shop1@test.com"
        };
        await AddAsync(shop1);

        var shop2 = new Shop
        {
            Name = "Shop 2",
            OwnerId = userId,
            ContactEmail = "shop2@test.com"
        };
        await AddAsync(shop2);

        var command = new CreateSharedDeliveryRunCommand
        {
            RunNumber = "RUN-001",
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop1.Id,
                    SequenceNumber = 1,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2)
                },
                new()
                {
                    ShopId = shop2.Id,
                    SequenceNumber = 2,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(3)
                }
            }
        };

        var runId = await SendAsync(command);

        var run = await FindAsync<SharedDeliveryRun>(runId);

        run.ShouldNotBeNull();
        run!.RunNumber.ShouldBe("RUN-001");
        run.Status.ShouldBe(DeliveryStatus.Pending);
        run.Stops.Count.ShouldBe(2);
    }

    [Test]
    public async Task ShouldRequireUniqueRunNumber()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop = new Shop
        {
            Name = "Test Shop",
            OwnerId = userId,
            ContactEmail = "test@shop.com"
        };
        await AddAsync(shop);

        var command1 = new CreateSharedDeliveryRunCommand
        {
            RunNumber = "RUN-DUP",
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop.Id,
                    SequenceNumber = 1,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2)
                }
            }
        };

        await SendAsync(command1);

        var command2 = new CreateSharedDeliveryRunCommand
        {
            RunNumber = "RUN-DUP", // Duplicate
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(2),
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop.Id,
                    SequenceNumber = 1,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(2).AddHours(2)
                }
            }
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command2));
    }

    [Test]
    public async Task ShouldRequireAtLeastOneStop()
    {
        var command = new CreateSharedDeliveryRunCommand
        {
            RunNumber = "RUN-EMPTY",
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            Stops = new List<DeliveryStopDto>() // Empty
        };

        await Should.ThrowAsync<ValidationException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldCalculateTotalDistance()
    {
        var userId = await RunAsDefaultUserAsync();

        var shop1 = new Shop
        {
            Name = "Shop 1",
            OwnerId = userId,
            ContactEmail = "shop1@test.com"
        };
        await AddAsync(shop1);

        var shop2 = new Shop
        {
            Name = "Shop 2",
            OwnerId = userId,
            ContactEmail = "shop2@test.com"
        };
        await AddAsync(shop2);

        var command = new CreateSharedDeliveryRunCommand
        {
            RunNumber = "RUN-DIST",
            ScheduledDate = DateTimeOffset.UtcNow.AddDays(1),
            TotalDistance = 25.5m, // 25.5 km
            Stops = new List<DeliveryStopDto>
            {
                new()
                {
                    ShopId = shop1.Id,
                    SequenceNumber = 1,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2)
                },
                new()
                {
                    ShopId = shop2.Id,
                    SequenceNumber = 2,
                    EstimatedArrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(3)
                }
            }
        };

        var runId = await SendAsync(command);

        var run = await FindAsync<SharedDeliveryRun>(runId);

        run.ShouldNotBeNull();
        run!.TotalDistance.ShouldBe(25.5m);
    }
}

