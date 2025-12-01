using Toss.Application.Common.Interfaces;
using Toss.Domain.Constants;
using Toss.Infrastructure.Authorization;
using Toss.Infrastructure.Data;
using Toss.Infrastructure.Data.Interceptors;
using Toss.Infrastructure.Identity;
using Toss.Infrastructure.Services.Authentication;
using Toss.Application.Common.Interfaces.Authentication;
using Toss.Application.Common.Interfaces.Tenancy;
using Toss.Infrastructure.Services.Tenancy;
using Toss.Infrastructure.Services.Operations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        RegisterIdentityServices(builder.Services, builder.Configuration);

        // Register infrastructure services (AI, User Management)
        RegisterInfrastructureServices(builder.Services);

        builder.Services.AddMemoryCache();

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
        services.AddScoped<ISaveChangesInterceptor, AuditSaveChangesInterceptor>();
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
    private static void RegisterIdentityServices(IServiceCollection services, IConfiguration configuration)   
    {
        // Configure Identity Core with roles first (needed for BearerToken)
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        // Configure authentication with both BearerToken and JWT Bearer schemes
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;                           
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddBearerToken(IdentityConstants.BearerScheme)
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"] ?? "TossErp",
                ValidAudience = jwtSettings["Audience"] ?? "TossErp",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured")))
            };
        });

        // Add authorization builder for policy configuration
        services.AddAuthorizationBuilder();

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

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IOtpSender, SmsOtpSender>();
        services.AddSingleton<ITwoFactorSessionStore, TwoFactorSessionStore>();
        services.AddScoped<IBusinessContext, BusinessContext>();
        services.AddScoped<IAuthorizationHandler, BusinessRoleAuthorizationHandler>();
        services.AddScoped<IOperationsTodayService, OperationsTodayService>();
        services.AddScoped<Toss.Application.Common.Interfaces.Manufacturing.IManufacturingCostingService,
            Toss.Infrastructure.Services.Manufacturing.ManufacturingCostingService>();
    }

    /// <summary>
    /// Configures authorization policies for the application.
    /// </summary>
    private static void ConfigureAuthorization(IServiceCollection services)     
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanPurge, policy =>
                policy.RequireRole(Roles.Administrator));
            
            // Global Identity role policies
            options.AddPolicy("RequireAdmin", policy =>
                policy.RequireRole(Roles.Administrator));
            
            options.AddPolicy("RequireRetailer", policy =>
                policy.RequireRole(Roles.Retailer));
            
            options.AddPolicy("RequireSupplier", policy =>
                policy.RequireRole(Roles.Supplier));
            
            options.AddPolicy("RequireDriver", policy =>
                policy.RequireRole(Roles.Driver));
            
            options.AddPolicy("RequireRetailerOrAdmin", policy =>
                policy.RequireRole(Roles.Retailer, Roles.Administrator));
            
            options.AddPolicy("RequireSupplierOrAdmin", policy =>
                policy.RequireRole(Roles.Supplier, Roles.Administrator));
            
            options.AddPolicy("RequireDriverOrAdmin", policy =>
                policy.RequireRole(Roles.Driver, Roles.Administrator));

            // Per-business role policies
            options.AddPolicy(Policies.RequireOwnerOrManager, policy =>
                policy.Requirements.Add(new BusinessRoleRequirement(new[]
                {
                    BusinessRoles.Owner,
                    BusinessRoles.Manager
                })));

            options.AddPolicy(Policies.RequirePosAccess, policy =>
                policy.Requirements.Add(new BusinessRoleRequirement(new[]
                {
                    BusinessRoles.Owner,
                    BusinessRoles.Manager,
                    BusinessRoles.Cashier
                })));

            options.AddPolicy(Policies.RequireStaffOrAbove, policy =>
                policy.Requirements.Add(new BusinessRoleRequirement(BusinessRoles.All)));
        });
    }
}
