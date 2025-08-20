using TossErp.Accounting.Application.Common.Helpers;
using Xunit;

namespace TossErp.Accounting.Application.Tests;

public class RoundingExtensionsTests
{
    [Theory]
    [InlineData(1.005m, 1.01m)]
    [InlineData(1.004m, 1.00m)]
    [InlineData(-1.005m, -1.01m)]
    [InlineData(-1.004m, -1.00m)]
    [InlineData(1234567.899m, 1234567.90m)]
    [InlineData(0m, 0m)]
    public void RoundMoney_WorksAsExpected(decimal input, decimal expected)
    {
        var result = input.RoundMoney();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundMoney_DoesNotMutateOriginal()
    {
        decimal original = 2.555m; // rounds to 2.56
        var result = original.RoundMoney();
        Assert.Equal(2.56m, result);
        Assert.Equal(2.555m, original); // ensure extension is pure
    }
}
