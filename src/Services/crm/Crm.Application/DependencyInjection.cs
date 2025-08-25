using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Crm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
