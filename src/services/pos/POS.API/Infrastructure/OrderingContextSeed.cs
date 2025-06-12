namespace eShop.POS.API.Infrastructure;

using eShop.POS.Domain.AggregatesModel.BuyerAggregate;

public class POSContextSeed: IDbSeeder<POSContext>
{
    public async Task SeedAsync(POSContext context)
    {
        if (!context.CardTypes.Any())
        {
            context.CardTypes.AddRange(GetPredefinedCardTypes());

            await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CardType> GetPredefinedCardTypes()
    {
        yield return new CardType { Id = 1, Name = "Amex" };
        yield return new CardType { Id = 2, Name = "Visa" };
        yield return new CardType { Id = 3, Name = "MasterCard" };
    }
}
