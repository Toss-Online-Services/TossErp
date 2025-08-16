using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using eShop.EventBus.Abstractions;
using TossErp.Stock.API.Services;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Application.EventHandlers;
using TossErp.Stock.Agent;

namespace TossErp.Stock.API;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        // Register integration event handlers
        builder.Services.AddScoped<IIntegrationEventHandler<eShop.EventBus.Events.Sales.SaleCompletedIntegrationEvent>, SaleCompletedIntegrationEventHandler>();
        builder.Services.AddScoped<IIntegrationEventHandler<eShop.EventBus.Events.Purchasing.PurchaseOrderReceivedIntegrationEvent>, PurchaseOrderReceivedIntegrationEventHandler>();

        // Register AI Agent services
        builder.Services.AddScoped<AICoPilotService>();
        builder.Services.AddScoped<GroupPurchaseAgent>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
