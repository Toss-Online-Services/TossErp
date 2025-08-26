using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Crm.Infrastructure.Data;
using Crm.Infrastructure.Data.Seeders;

namespace TestDataSeeder;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup service collection
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add DbContext
        services.AddDbContext<CrmDbContext>(options =>
            options.UseNpgsql("Host=localhost;Port=5432;Database=tosserpdb;Username=postgres;Password=postgres123;"));

        // Add the data seeder
        services.AddScoped<CrmDataSeeder>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var seeder = scope.ServiceProvider.GetRequiredService<CrmDataSeeder>();

            logger.LogInformation("Starting CRM data seeding test...");

            await seeder.SeedAsync();

            logger.LogInformation("CRM data seeding test completed successfully!");
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error during data seeding test");
            throw;
        }
    }
}
