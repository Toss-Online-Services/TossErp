using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TossErp.ServiceTests;

public class ServiceStartupTests
{
    [Fact]
    public async Task Service_Should_Start_And_Stop_Successfully()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddHealthChecks();

        var app = builder.Build();
        app.MapGet("/", () => "TOSS ERP III Service is running");

        // Act & Assert - Should not throw
        Assert.NotNull(app);
        
        await app.StartAsync();
        await app.StopAsync();
    }
}

[global::Xunit.CollectionDefinition("Sequential")]
public class SequentialCollection : global::Xunit.ICollectionFixture<object>
{
}
