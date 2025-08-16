using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TossErp.Stock.Domain.Constants;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using TossErp.Stock.Infrastructure.Identity;
using TossErp.Stock.Infrastructure.Data.Interceptors;
using TossErp.Stock.Infrastructure.Repositories;
using TossErp.Stock.Infrastructure.Services;
using TossErp.Stock.Domain.Common;
using TossErp.Stock.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using eShop.EventBus.Extensions;
using eShop.EventBus.Services;
using eShop.EventBus.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TossErp.Stock.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TossErpDb");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register the interface with the concrete implementation
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        // Add Identity services with proper configuration
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Add Authorization services (required for IAuthorizationService)
        services.AddAuthorization(options =>
        {
            // Add default authorization policy
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // Add Email services for Identity API - register as scoped to avoid root provider issues
        services.AddScoped<IEmailSender<ApplicationUser>, EmailSender>();

        // Register Identity service
        services.AddTransient<IIdentityService, IdentityService>();

        // Register repositories
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IStockLevelRepository, StockLevelRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IStockEntryRepository, StockEntryRepository>();
        services.AddScoped<IStockLedgerEntryRepository, StockLedgerEntryRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IBinRepository, BinRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
        services.AddScoped<ISerialNoRepository, SerialNoRepository>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
        services.AddScoped<IStockEntryTypeRepository, StockEntryTypeRepository>();

        // Register generic repository for all entities
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register MassTransit Event Bus
        services.AddMassTransitEventBus(configuration);

        // Register Domain Event Dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register Event Bus Health Check
        services.AddHealthChecks()
            .AddCheck<EventBusHealthCheck>("eventbus", tags: new[] { "eventbus", "messaging" });

        // Register HttpClient for external services
        services.AddHttpClient<IBarcodeService, BarcodeService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "TossErp-Stock-Service");
        });

        services.AddHttpClient<ITaxCalculationService, TaxCalculationService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "TossErp-Stock-Service");
        });

        // Register external service implementations
        services.AddScoped<IBarcodeService, BarcodeService>();
        services.AddScoped<ITaxCalculationService, TaxCalculationService>();

        // Register AI Agent services from the Agent project
        services.AddScoped<TossErp.Stock.Agent.AICoPilotService>();
        services.AddScoped<TossErp.Stock.Agent.GroupPurchaseAgent>();

        return services;
    }
}
