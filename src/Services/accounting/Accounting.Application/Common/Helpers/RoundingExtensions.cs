namespace TossErp.Accounting.Application.Common.Helpers;

public static class RoundingExtensions
{
    public static decimal RoundMoney(this decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
