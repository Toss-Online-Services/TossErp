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
        // Add Entity Framework
        services.AddDbContext<CrmDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to in-memory database
                options.UseInMemoryDatabase("CrmDb");
            }
            else
            {
                options.UseNpgsql(connectionString, b =>
                {
                    b.MigrationsAssembly("Crm.Infrastructure");
                });
            }
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
