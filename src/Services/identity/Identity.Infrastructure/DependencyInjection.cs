using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<IUserConsentRepository, InMemoryUserConsentRepository>();
        services.AddScoped<IAuditTrailRepository, InMemoryAuditTrailRepository>();

        // Register services
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
