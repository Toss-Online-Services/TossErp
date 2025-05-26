using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Infrastructure;
using Ordering.API.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Infrastructure;

public class OrderingContextSeed
{
    public async Task SeedAsync(OrderingContext context, ILogger<OrderingContextSeed> logger)
    {
        try
        {
            if (!context.CardTypes.Any())
            {
                context.CardTypes.AddRange(GetPredefinedCardTypes());

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(OrderingContext));
            throw;
        }
    }

    private static IEnumerable<CardType> GetPredefinedCardTypes()
    {
        return new List<CardType>
        {
            CardType.Amex,
            CardType.Visa,
            CardType.MasterCard
        };
    }
}
