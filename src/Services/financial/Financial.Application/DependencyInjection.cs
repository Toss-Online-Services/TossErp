using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Financial.Application;

/// <summary>
/// Dependency injection configuration for Financial Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Financial Application services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddFinancialApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Add AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
