using Crm.Domain.Repositories;
using Crm.Infrastructure.Data;
using Crm.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Use in-memory repository for now to avoid EF dependencies
        services.AddSingleton<ICustomerRepository, Crm.Infrastructure.Repositories.InMemoryCustomerRepository>();
        
        // TODO: Add other repository interfaces when Analytics and Sales repositories are implemented
        // services.AddSingleton<IAnalyticsRepository, InMemoryAnalyticsRepository>();
        // services.AddSingleton<ISalesRepository, InMemorySalesRepository>();

        return services;
    }
}
