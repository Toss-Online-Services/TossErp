using Toss.Application.Common.Interfaces;
using Toss.Domain.Constants;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.Data.Interceptors;
using Toss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Infrastructure layer dependency injection configuration.
/// Registers database context, interceptors, identity services, and infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds all infrastructure services to the host application builder.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <exception cref="ArgumentNullException">Thrown when connection string 'TossDb' is not found.</exception>
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Validate connection string exists
        var connectionString = builder.Configuration.GetConnectionString("TossDb");
        Guard.Against.Null(connectionString, message: "Connection string 'TossDb' not found.");

        // Register EF Core interceptors for cross-cutting concerns
        RegisterInterceptors(builder.Services);

        // Configure and register the database context
        RegisterDbContext(builder, connectionString);

        // Register Identity and Authentication services
        RegisterIdentityServices(builder.Services);

        // Register infrastructure services (AI, User Management)
        RegisterInfrastructureServices(builder.Services);

        // Configure authorization policies
        ConfigureAuthorization(builder.Services);
    }

    /// <summary>
    /// Registers all EF Core save changes interceptors.
    /// </summary>
    private static void RegisterInterceptors(IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DateTimeOffsetInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DateTimeInterceptor>();
    }

    /// <summary>
    /// Configures and registers the ApplicationDbContext with PostgreSQL provider.
    /// </summary>
    private static void RegisterDbContext(IHostApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // Add all registered interceptors
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            
            // Configure PostgreSQL provider
            options.UseNpgsql(connectionString);
            
            // Suppress pending model changes warning (useful during development)
            options.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        // Enrich with Aspire observability if available
        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        // Register context interface for clean architecture
        builder.Services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Register database initializer
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
    }

    /// <summary>
    /// Registers ASP.NET Core Identity and authentication services.
    /// </summary>
    private static void RegisterIdentityServices(IServiceCollection services)
    {
        // Configure bearer token authentication
        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        // Add authorization builder for policy configuration
        services.AddAuthorizationBuilder();

        // Configure Identity Core with roles
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        // Register time provider
        services.AddSingleton(TimeProvider.System);
        
        // Register identity services
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IUserManagementService, Toss.Infrastructure.Identity.Services.UserManagementService>();
    }

    /// <summary>
    /// Registers infrastructure services for AI and other features.
    /// </summary>
    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        // AI Services
        services.AddScoped<Toss.Infrastructure.Services.ArtificialIntelligence.IAISettingsService, 
            Toss.Infrastructure.Services.ArtificialIntelligence.AISettingsService>();
        
        services.AddHttpClient<Toss.Infrastructure.Services.ArtificialIntelligence.ArtificialIntelligenceHttpClient>();
        
        services.AddScoped<Toss.Application.Common.Interfaces.IArtificialIntelligenceService, 
            Toss.Infrastructure.Services.ArtificialIntelligence.ArtificialIntelligenceService>();
    }

    /// <summary>
    /// Configures authorization policies for the application.
    /// </summary>
    private static void ConfigureAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => 
                policy.RequireRole(Roles.Administrator)));
    }
}
