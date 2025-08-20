using Crm.Domain.Repositories;
using Crm.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddScoped<ICustomerInteractionRepository, InMemoryCustomerInteractionRepository>();
        services.AddScoped<ILoyaltyTransactionRepository, InMemoryLoyaltyTransactionRepository>();

        return services;
    }
}
