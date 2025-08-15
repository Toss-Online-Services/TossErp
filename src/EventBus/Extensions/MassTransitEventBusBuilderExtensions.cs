#nullable enable

using eShop.EventBus.Abstractions;
using eShop.EventBus.Configuration;
using eShop.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace eShop.EventBus.Extensions;

public static class MassTransitEventBusBuilderExtensions
{
    public static IServiceCollection AddMassTransitEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureMassTransit = null)
    {
        var eventBusConfig = new EventBusConfiguration
        {
            EventBusConnection = configuration["EventBus:EventBusConnection"] ?? "amqp://guest:guest@localhost:5672",
            EventBusUserName = configuration["EventBus:EventBusUserName"] ?? "guest",
            EventBusPassword = configuration["EventBus:EventBusPassword"] ?? "guest",
            EventBusRetryCount = configuration["EventBus:EventBusRetryCount"] ?? "5",
            SubscriptionClientName = configuration["EventBus:SubscriptionClientName"] ?? "DefaultService"
        };

        services.AddMassTransit(x =>
        {
            // Configure consumers
            x.AddConsumers(typeof(MassTransitEventBusBuilderExtensions).Assembly);

            // Configure RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(eventBusConfig.EventBusConnection), host =>
                {
                    host.Username(eventBusConfig.EventBusUserName);
                    host.Password(eventBusConfig.EventBusPassword);
                });

                // Configure retry policy
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(int.Parse(eventBusConfig.EventBusRetryCount), TimeSpan.FromSeconds(2));
                });

                // Configure error handling - use the new API
                cfg.UseInMemoryOutbox(context);

                // Configure consumers
                cfg.ConfigureEndpoints(context);
            });

            // Allow custom configuration
            configureMassTransit?.Invoke(x);
        });

        // Register EventBus implementation
        services.AddScoped<IEventBus, EventBus>();

        return services;
    }

    public static IServiceCollection AddIntegrationEventHandlers<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services)
        where T : class, IIntegrationEventHandler
    {
        services.AddScoped<T>();
        return services;
    }

    public static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services, params Type[] handlerTypes)
    {
        foreach (var handlerType in handlerTypes)
        {
            if (typeof(IIntegrationEventHandler).IsAssignableFrom(handlerType))
            {
                services.AddScoped(handlerType);
            }
        }
        return services;
    }
}
