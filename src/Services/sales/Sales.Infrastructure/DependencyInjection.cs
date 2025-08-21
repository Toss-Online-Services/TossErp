using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Infrastructure.Data;
using TossErp.Sales.Infrastructure.Repositories;

namespace TossErp.Sales.Infrastructure;

/// <summary>
/// Dependency injection configuration for Sales Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Database
        services.AddDbContext<SalesDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "sales");
                npgsqlOptions.EnableRetryOnFailure(3);
            });

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Repositories
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ITillRepository, TillRepository>();

        // Health checks
        services.AddHealthChecks()
            .AddDbContextCheck<SalesDbContext>("sales-database", tags: new[] { "database", "sales" });

        return services;
    }

    public static async Task<IServiceProvider> MigrateSalesDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
        
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Log migration error
            Console.WriteLine($"Sales database migration failed: {ex.Message}");
            throw;
        }

        return serviceProvider;
    }
}
