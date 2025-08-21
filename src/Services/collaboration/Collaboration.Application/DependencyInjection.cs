namespace Collaboration.Application;

using Microsoft.Extensions.DependencyInjection;
using MediatR;

public static class DependencyInjection
{
    public static IServiceCollection AddCollaborationApplication(this IServiceCollection services)
    {
        // Add MediatR handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
