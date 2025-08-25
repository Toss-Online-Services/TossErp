using Crm.Domain.Repositories;
using Crm.Infrastructure.Data;
using Crm.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework - simplified for now
        services.AddDbContext<CrmDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("TossErpDb");
            // TODO: Configure PostgreSQL once packages are properly restored
            // options.UseNpgsql(connectionString);
            
            // For now, use in-memory database to avoid package issues
            options.UseInMemoryDatabase("CrmDb");
        });

        // TODO: Add health checks once packages are restored
        // services.AddHealthChecks()
        //     .AddDbContextCheck<CrmDbContext>("crm-database");

        // Register repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }

    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CrmDbContext>>();

        try
        {
            logger.LogInformation("Starting CRM database migration...");
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("CRM database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the CRM database");
            throw;
        }
    }
}
