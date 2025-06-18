using eShop.EventBusRabbitMQ;
using eShop.IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using POS.Infrastructure;

namespace POS.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // Register EventBus
        builder.AddRabbitMqEventBus("DefaultConnection");

        // Register IntegrationEventLogService
        builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<POSContext>>();
    }

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        ((IHostApplicationBuilder)builder).AddApplicationServices();
    }
} 
