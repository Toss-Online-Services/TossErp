using eShop.EventBus.Abstractions;
using eShop.EventBus.Configuration;
using eShop.EventBus.Events;
using MassTransit;
using MassTransit.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eShop.EventBus.Extensions;

public static class MassTransitEventBusBuilderExtensions
{
    public static IServiceCollection AddMassTransitEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureMassTransit = null)
    {
        var eventBusConfig = new EventBusConfiguration();
        configuration.GetSection("EventBus").Bind(eventBusConfig);

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

                // Configure message serialization
                cfg.UseNewtonsoftJson(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });

                // Configure retry policy
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(int.Parse(eventBusConfig.EventBusRetryCount), TimeSpan.FromSeconds(2));
                });

                // Configure error handling
                cfg.UseInMemoryOutbox();

                // Configure consumers
                cfg.ConfigureEndpoints(context);

                // Configure message topology
                cfg.Message<IntegrationEvent>(e =>
                {
                    e.SetEntityName("integration-events");
                });
            });

            // Allow custom configuration
            configureMassTransit?.Invoke(x);
        });

        // Register EventBus implementation
        services.AddScoped<IEventBus, EventBus>();

        return services;
    }

    public static IServiceCollection AddIntegrationEventHandlers<T>(this IServiceCollection services)
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
